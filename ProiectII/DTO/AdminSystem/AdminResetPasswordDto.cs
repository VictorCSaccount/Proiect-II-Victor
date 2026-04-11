namespace ProiectII.DTO.AdminSystem
{
    public class AdminResetPasswordDto
    {
        public uint TargetUserId { get; set; }
        public string NewPassword { get; set; } = string.Empty;
    }
}
