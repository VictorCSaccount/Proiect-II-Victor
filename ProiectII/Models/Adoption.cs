using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class Adoption
    {
        [Key]
        public uint Id { get; set; }
        public uint FoxId { get; set; }
        public Fox Fox { get; set; }

        public uint UserId { get; set; }
        public ApplicationUser User { get; set; }
        public AdoptionStatus AdoptionStatus { get; set; }
        public DateTime RequestDate { get; set; }

        public string? Reason { get; set; } = null;




        public void ApproveAdoption()
        {
            AdoptionStatus = AdoptionStatus.Approved;
            Fox.Adopt();
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

