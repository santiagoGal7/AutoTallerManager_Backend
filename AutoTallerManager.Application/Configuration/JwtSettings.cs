namespace AutoTallerManager.Application.Configuration;

public class JwtSettings
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = "AutoTallerManagerApi";
    public string Audience { get; set; } = "AutoTallerManagerClients";
    public double ExpiryInMinutes { get; set; } = 180;
}
