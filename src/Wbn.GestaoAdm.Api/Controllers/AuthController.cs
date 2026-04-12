using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Wbn.GestaoAdm.Api.Authentication;
using Wbn.GestaoAdm.Api.Contracts.Auth;
using Wbn.GestaoAdm.Application.Modules.Auth.Dtos;
using Wbn.GestaoAdm.Application.Modules.Auth.Interfaces;

namespace Wbn.GestaoAdm.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AuthController(
    IAuthAppService authAppService,
    IOptions<JwtOptions> jwtOptions) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken)
    {
        try
        {
            var authenticatedUser = await authAppService.AuthenticateAsync(
                new AuthenticateUserRequest(request.EmpresaId, request.Email, request.Senha),
                cancellationToken);

            var expiresAt = DateTime.UtcNow.AddMinutes(jwtOptions.Value.ExpirationInMinutes);
            var token = GenerateToken(authenticatedUser, expiresAt);

            return Ok(new LoginResponse(
                token,
                expiresAt,
                new AuthenticatedUserResponse(
                    authenticatedUser.Id,
                    authenticatedUser.PerfilId,
                    authenticatedUser.Nome,
                    authenticatedUser.Email,
                    authenticatedUser.Telefone,
                    authenticatedUser.Ativo,
                    authenticatedUser.UltimoLogin),
                new EmpresaSessaoResponse(
                    authenticatedUser.Empresa.Id,
                    authenticatedUser.Empresa.NomeFantasia,
                    authenticatedUser.Empresa.RazaoSocial,
                    authenticatedUser.Empresa.Cnpj,
                    authenticatedUser.Empresa.Ativo)));
        }
        catch (InvalidOperationException ex)
        {
            return Unauthorized(new { message = ex.Message });
        }
    }

    private string GenerateToken(AuthenticatedUserResult authenticatedUser, DateTime expiresAt)
    {
        var options = jwtOptions.Value;
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(options.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, authenticatedUser.Id.ToString()),
            new Claim(ClaimTypes.NameIdentifier, authenticatedUser.Id.ToString()),
            new Claim(ClaimTypes.Name, authenticatedUser.Nome),
            new Claim(ClaimTypes.Email, authenticatedUser.Email),
            new Claim("codigoUsuario", authenticatedUser.Id.ToString()),
            new Claim("perfilId", authenticatedUser.PerfilId.ToString()),
            new Claim("empresaId", authenticatedUser.Empresa.Id.ToString())
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
