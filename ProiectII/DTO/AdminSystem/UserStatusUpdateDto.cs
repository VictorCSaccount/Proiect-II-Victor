namespace ProiectII.DTO.AdminSystem
{
    public class UserStatusUpdateDto
    {
        public uint UserId { get; set; }
        public bool IsActive { get; set; }
        public string? Reason { get; set; }
    }
}
