namespace ProiectII.DTO.CommentsReport
{
  
        public class CommentModerationDto : CommentDto

        {

            public string UserEmail { get; set; } = string.Empty;

            public bool IsVisible { get; set; }

            public int ReportsCount { get; set; }

        }
    
}
