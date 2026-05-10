using System.ComponentModel.DataAnnotations;

namespace ProiectII.DTO.Reviews
{
    public class CreateAppReviewDto
    {
        [Range(1, 5, ErrorMessage = "Nota trebuie să fie între 1 și 5.")]
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
