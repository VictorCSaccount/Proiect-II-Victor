using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectII.Models
{
    public class SecurityLog
    {
        [Key]
        public uint Id { get; set; }
        [Required]
        [MaxLength(255)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; } // Relația cu cine a făcut acțiunea

        [Required]
        public ActionType Action { get; set; } // Aici folosim Enum-ul de mai sus

        [Required]
        [MaxLength(500)]
        public string Details { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}