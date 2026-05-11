using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wbn.GestaoAdm.Api.Authentication;
using Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Dtos;
using Wbn.GestaoAdm.Application.Modules.Mobile.Auth.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers.Mobile;

[ApiController]
[Route("api/mobile/auth")]
public sealed class MobileAuthController(
    IMobileAuthAppService authAppService,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(MobileLoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<MobileLoginResponse>> Login(
        [FromBody] MobileLoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var result = await authAppService.AuthenticateAsync(request, cancellationToken);

            var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpirationInMinutes);
            var token = GerarToken(result, expiresAt);

            return Ok(new MobileLoginResponse(
                token,
                expiresAt,
                result.Id,
                result.Nome,
                result.Email,
                result.Telefone,
                result.Empresas));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    private string GerarToken(MobileLoginResult result, DateTime expiresAt)
    {
        var options = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, result.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, result.Id.ToString()),
            new Claim(ClaimTypes.Name, result.Nome),
            new Claim(ClaimTypes.Email, result.Email),
            new Claim("codigoUsuario", result.Id.ToString()),
            new Claim("perfilId", result.PerfilId.ToString()),
            new Claim("origem", "mobile")
        };

        var token = new JwtSecurityToken(
            issuer: options.Issuer,
            audience: options.Audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public sealed record MobileLoginResponse(
    string Token,
    DateTime ExpiresAt,
    ulong UsuarioId,
    string Nome,
    string Email,
    string? Telefone,
    IReadOnlyCollection<MobileEmpresaVinculadaResult> Empresas);
