using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectII.Models
{
    public class Adoption
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        public uint FoxId { get; set; }

        [ForeignKey("FoxId")]
        public Fox Fox { get; set; }

        [Required]
        [MaxLength(255)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public AdoptionStatus AdoptionStatus { get; set; }
        public DateTime RequestDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string? Reason { get; set; } = null;




        public void ApproveAdoption(uint adoptedStatusId)
        {
            this.AdoptionStatus = AdoptionStatus.Approved;

            if (this.Fox != null)
            {
                this.Fox.Adopt(adoptedStatusId);
            }
        }

        public void RejectAdoption(string reason)
        {
            AdoptionStatus = AdoptionStatus.Rejected;
            Reason = reason;
        }


        public void CancelAdoption()
        {
            AdoptionStatus = AdoptionStatus.CanceledByUser;
        }


        public bool IsExpired()
        {
            ///daca l user sta in pending mai mmult de cateva zile atunci adoptia expira si poate fi preluata de alt user
            if (AdoptionStatus != AdoptionStatus.Pending) return false;

            const int ExpirationDays = 14;
            return DateTime.UtcNow > RequestDate.AddDays(ExpirationDays);
        }

    }

}

