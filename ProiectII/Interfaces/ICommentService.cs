using ProiectII.DTO.CommentsReport;

namespace ProiectII.Interfaces
{
    public interface ICommentService
    {
        // cand se lucreaza pe servicii in general folosim task.. petru a nu opri executia...
        //deoarece se lucreaza cu servere baze de date etc.. si aetea tin timp...

        //implementez task-urile unui user obisnuit pentur un comentariu


        Task<IEnumerable<CommentDto>> GetCommentsByFoxIdAsync(uint foxId); // returnam de la fox toate comentariil
        Task<IEnumerable<CommentDto>> GetCommentsByUserIdAsync(string userId);       // retunram de la user toate comantariile

        Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, string userId); // creem un comentariu nou (facut de un utilizator

        Task<bool> EditCommentAsync(uint commentId, UpdateCommentDto dto, string userId);

        Task<bool> DeleteCommentAsync(uint commentId, string userId); // user ul isi poate sterge comentariul


        //adinistratorul poate sterge comentariul sau sal ascunda...

        Task<bool> AdminHideComment(uint commentId);

        Task<bool> AdminUnhideComment (uint commentId);



        Task<bool> AdminDeleteCommentAsync(uint commentId); // administratorul poate sterge comentariul




    }
}
