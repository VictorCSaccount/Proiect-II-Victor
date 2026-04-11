namespace ProiectII.DTO.CommentsReport
{
    public class CommentDto
    {
        public uint Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public bool IsEdited { get; set; }
        public DateTime? EditedAt { get; set; }
    }
}
