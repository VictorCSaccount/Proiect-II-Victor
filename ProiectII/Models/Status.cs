using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class Status
    {
        [Key]
        public uint Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [StringLength(250)]
        public string Description { get; set; }

        public bool IsAdoptable { get; set; } // Indică dacă vulpea este adoptabilă sau nu, în funcție de starea sa actuală și de alte criterii relevante
        public FoxStatus FoxStatus { get; set; }

        public bool VerifyIsAdoptable()
        {
            if (!IsAdoptable) return false;

            if (this.FoxStatus == FoxStatus.Deceased)
            {
                return false;
            }

            return FoxStatus switch
            {
                FoxStatus.Quarantined => false,
                FoxStatus.MedicalTreatment => false,
                FoxStatus.LegalHold => false,
                FoxStatus.Adopted => false,
                _ => true
            };
        }

        public bool VerifyIsIsolated()
        {
            if (this.FoxStatus == FoxStatus.Quarantined || this.FoxStatus == FoxStatus.MedicalTreatment)
            {
                return true;
            }
            return false;
        }


        public void UpdateAdoptability()
        {
            if (VerifyIsIsolated() || this.FoxStatus == FoxStatus.Deceased || IsRestricted())
            {
                this.IsAdoptable = false;
                return;
            }


            if (this.FoxStatus == FoxStatus.Healthy)
            {
                this.IsAdoptable = true;
            }
        }


        public bool IsRestricted()
        {
            return this.FoxStatus == FoxStatus.LegalHold;
        }

        public void ChangeStatus(FoxStatus newStatus)
        {
            this.FoxStatus = newStatus;
            UpdateAdoptability();
        }

    }
}
