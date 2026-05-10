using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class AppReview
    {
        [Key]
        public uint Id { get; set; }

        // Nu este required, poate fi lăsat de un vizitator anonim
        public string? UserId { get; set; }
        public ApplicationUser? User { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string Comment { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}