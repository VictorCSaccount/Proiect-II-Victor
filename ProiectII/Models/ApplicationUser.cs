using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    // Moștenim IdentityUser<string> pentru că ai vrut ID-uri de tip string 
    public class ApplicationUser : IdentityUser<string>
    {
        // IdentityUser are deja: Id, UserName, Email, PasswordHash, PhoneNumber

        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; }

        [Required]
        [MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        public DateOnly BornDate { get; set; }

        [MaxLength(512)] // Sincronizat cu ERD
        public string? ProfilePictureUrl { get; set; }

        public bool IsActive { get; set; } = true;

        public string? DeactivationReason { get; set; } = null;

        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        //  Relatii !!!! (1:N)

        public virtual ICollection<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();
        public virtual ICollection<Adoption> Adoptions { get; set; } = new List<Adoption>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public virtual ICollection<Report> ReportsCreated { get; set; } = new List<Report>();



        public int GetAge()
        {
            var today = DateOnly.FromDateTime(DateTime.UtcNow);
            int age = today.Year - BornDate.Year;
            if (BornDate > today.AddYears(-age)) age--;
            return age;
        }

        public bool IsAdult() => GetAge() >= 18;

        public void UpdateLastLogin()
        {
            LastLogin = DateTime.UtcNow;
        }

        public void Deactivate(string reason)
        {
            IsActive = false;
            DeactivationReason = reason; 


        }

        public string IsActiveStatus()
        {

            if (IsActive)
            {
                return "Active";
            }
            else
            {
                return $"Inactive - Reason: {DeactivationReason}";
            }

        }


    }

    
}