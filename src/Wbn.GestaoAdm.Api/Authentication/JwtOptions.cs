namespace Wbn.GestaoAdm.Api.Authentication;

public sealed class JwtOptions
{
    public const string SectionName = "Jwt";

    public string Issuer { get; init; } = "Wbn.GestaoAdm.Api";
    public string Audience { get; init; } = "Wbn.GestaoAdm.Frontend";
    public string SecretKey { get; init; } = string.Empty;
    public int ExpirationInMinutes { get; init; } = 120;
}
