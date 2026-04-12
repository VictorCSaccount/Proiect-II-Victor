using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProiectII.Models
{
    public class Comment
    {
    

        [Key]
        public uint Id { get; set; }
        [Required]
        [MaxLength(500)]
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        [Required]
        [MaxLength(255)]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        [Required]
        public uint FoxId { get; set; }

        [ForeignKey("FoxId")]
        public Fox Fox { get; set; } 

        public bool IsDeleted { get; set; } = false;

        public void SoftDelete()
            {
            IsDeleted = true;
            UpdatedAt = DateTime.Now;
        }

        public void EditContent(string newContent)
        {
            Content = newContent;
            UpdatedAt = DateTime.Now;
        }

        public bool IsValidLength()
        {
            return !string.IsNullOrWhiteSpace(Content) && Content.Length <= 500;
        }



    }
}
