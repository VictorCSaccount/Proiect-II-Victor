using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectII.Models
{
    public class Report
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        [Column(TypeName = "text")]
        public string Description { get; set; } = string.Empty;

        [Required]
        [MaxLength(450)]

        public string UserId { get; set; }

        [ForeignKey("ReporterId")]
        public virtual ApplicationUser? Reporter { get; set; }

        [Required]
        public uint LocationId { get; set; }

        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }
        
        [MaxLength(512)]
        public string? ImageUrl { get; set; }

        public ReportStatus Status { get; set; } = ReportStatus.Pending;

        public void Resolve()
        {
            if (Status == ReportStatus.Resolved)
            {
                return;
            }

            Status = ReportStatus.Resolved;

        }
    }
}