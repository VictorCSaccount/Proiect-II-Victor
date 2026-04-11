using System.ComponentModel.DataAnnotations;

namespace ProiectII.Models
{
    public class Comment
    {
    

        [Key]
        public uint Id { get; set; }
        public string Content { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        public uint UserId { get; set; } 
        public ApplicationUser User { get; set; }
        public uint FoxId { get; set; } 
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
