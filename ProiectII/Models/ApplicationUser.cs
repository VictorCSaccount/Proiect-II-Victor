using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    // Moștenim IdentityUser<uint> pentru că ai vrut ID-uri de tip uint (fără semn)
    public class ApplicationUser : IdentityUser<uint>
    {
        // IdentityUser are deja: Id, UserName, Email, PasswordHash, PhoneNumber

        [Required]
        public DateOnly BornDate { get; set; }

        [MaxLength(255)]
        public string? ProfilePictureUrl { get; set; }
        public bool IsActive { get; set; }

        public string? DeactivationReason { get; set; } = null;

        public DateTime LastLogin { get; set; } = DateTime.UtcNow;

        //  cu SecurityLogs (1:N)
        public virtual ICollection<SecurityLog> SecurityLogs { get; set; } = new List<SecurityLog>();

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