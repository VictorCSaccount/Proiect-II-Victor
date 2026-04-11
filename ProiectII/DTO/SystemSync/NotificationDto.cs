namespace ProiectII.DTO.SystemSync
{
    public class NotificationDto
    {
        public uint Id { get; set; }

        public string Message { get; set; } = string.Empty;

        // ex: "Info", "Warning", "AdoptionUpdate"
        public string Type { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }

        public bool IsRead { get; set; }
    }
}
