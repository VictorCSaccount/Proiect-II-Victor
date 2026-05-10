namespace ProiectII.DTO.AdminSystem
{
    public class UserSelectionDto
    {
        public string Id { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}