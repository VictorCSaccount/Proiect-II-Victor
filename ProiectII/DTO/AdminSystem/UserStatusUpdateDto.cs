namespace ProiectII.DTO.AdminSystem
{
    public class UserStatusUpdateDto
    {
        public string UserId { get; set; }
        public bool IsActive { get; set; }
        public string? Reason { get; set; }
    }
}
