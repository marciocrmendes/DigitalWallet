namespace DigitalWallet.CrossCutting.Options
{
    public class JwtSettings
    {
        public required string Issuer { get; set; }
        public required string Audience { get; set; }
        public required string TokenSecurityKey { get; set; }
        public required int TokenExpirationInMinutes { get; set; } = 60;
    }
}
