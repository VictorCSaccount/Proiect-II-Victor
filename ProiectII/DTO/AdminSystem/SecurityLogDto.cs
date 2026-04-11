namespace ProiectII.DTO.AdminSystem
{
    public class SecurityLogDto
    {
        public DateTime Timestamp { get; set; }
        public string UserEmail { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public string Severity { get; set; } = "Info"; // se armonizeaza cu cel din clasa...
    }
}
