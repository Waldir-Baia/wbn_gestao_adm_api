namespace Wbn.GestaoAdm.Api.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; set; } = "Wbn.GestaoAdm.Api";
    public string Audience { get; set; } = "Wbn.GestaoAdm.Frontend";
    public string SecretKey { get; set; } = string.Empty;
    public int ExpirationInMinutes { get; set; } = 120;
}
