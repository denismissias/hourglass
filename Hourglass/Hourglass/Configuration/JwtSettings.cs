namespace Hourglass.Configuration;

/// <summary>
/// JWT configuration settings loaded from appsettings.json
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key for signing JWT tokens
    /// </summary>
    public required string Secret { get; set; }

    /// <summary>
    /// Token expiration time in hours
    /// </summary>
    public int ExpirationHours { get; set; } = 2;

    /// <summary>
    /// Issuer claim value
    /// </summary>
    public string Issuer { get; set; } = "Hourglass";
}
