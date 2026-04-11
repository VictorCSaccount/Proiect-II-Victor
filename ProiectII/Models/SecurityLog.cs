using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class SecurityLog
    {
        [Key]
        public uint Id { get; set; }
        public uint UserId { get; set; }
        public virtual ApplicationUser User { get; set; } // Relația cu cine a făcut acțiunea

        public ActionType Action { get; set; } // Aici folosim Enum-ul de mai sus
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}