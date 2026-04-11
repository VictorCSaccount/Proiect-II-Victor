using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectII.Models
{
    public class Report
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        public string Description { get; set; } = string.Empty;
        public uint ReporterId { get; set; }
        [ForeignKey("ReporterId")]
        public virtual ApplicationUser? Reporter { get; set; }
        public uint LocationId { get; set; }
        [ForeignKey("LocationId")]
        public virtual Location? Location { get; set; }
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