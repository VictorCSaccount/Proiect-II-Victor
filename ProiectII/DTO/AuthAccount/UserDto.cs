namespace ProiectII.DTO.AuthAccount
{
    public class UserDto
    {
        public string Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateOnly BirthDate { get; set; }
        public string? ProfilePictureUrl { get; set; }
    }
}
