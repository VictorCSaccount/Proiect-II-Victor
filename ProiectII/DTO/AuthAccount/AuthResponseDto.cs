namespace ProiectII.DTO.AuthAccount
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string UserRole { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
    }
}
