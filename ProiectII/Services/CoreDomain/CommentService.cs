using AutoMapper;
using ProiectII.DTO.CommentsReport;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Services.CoreDomain
{
    public class CommentService : ICommentService
    {
        private readonly ICommentRepository _repository;
        private readonly IMapper _mapper;

        public CommentService(ICommentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByFoxIdAsync(uint foxId)
        {
            var comments = await _repository.GetCommentsByFoxIdAsync(foxId);
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<IEnumerable<CommentDto>> GetCommentsByUserIdAsync(string userId)
        {
            var comments = await _repository.GetCommentsByUserIdAsync(userId);
            return _mapper.Map<IEnumerable<CommentDto>>(comments);
        }

        public async Task<CommentDto> CreateCommentAsync(CreateCommentDto dto, string userId)
        {
            var commentEntity = _mapper.Map<Comment>(dto);

            // Setarile care NU vin din DTO
            commentEntity.UserId = userId;
            commentEntity.CreatedAt = DateTime.UtcNow;
            commentEntity.UpdatedAt = null;
            commentEntity.IsDeleted = false;
            commentEntity.IsVisible = true; // Implicit, e vizibil cand e postat

            await _repository.AddAsync(commentEntity);
            await _repository.SaveChangesAsync(); // CRITIC: Executia salvarii

            return _mapper.Map<CommentDto>(commentEntity);
        }

        public async Task<bool> EditCommentAsync(uint commentId, UpdateCommentDto dto, string userId)
        {
            var comment = await _repository.GetByIdAsync(commentId);

            if (comment == null || comment.UserId != userId || comment.IsDeleted)
            {
                return false;
            }

            comment.Content = dto.EditedContent;
            comment.UpdatedAt = DateTime.UtcNow;

            _repository.Update(comment);
            return await _repository.SaveChangesAsync(); // CRITIC: Salvare in DB
        }

        public async Task<bool> DeleteCommentAsync(uint commentId, string userId)
        {
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null || comment.UserId != userId)
            {
                return false;
            }

            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;

            _repository.Update(comment);
            return await _repository.SaveChangesAsync();
        }

        // --- FUNCTII ADMIN ---

        public async Task<bool> AdminHideComment(uint commentId)
        {
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null) return false;

            comment.IsVisible = false;
            comment.UpdatedAt = DateTime.UtcNow;

            _repository.Update(comment);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> AdminUnhideComment(uint commentId)
        {
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null) return false;

            comment.IsVisible = true;
            comment.UpdatedAt = DateTime.UtcNow;

            _repository.Update(comment);
            return await _repository.SaveChangesAsync();
        }

        public async Task<bool> AdminDeleteCommentAsync(uint commentId)
        {
            var comment = await _repository.GetByIdAsync(commentId);
            if (comment == null) return false;

            comment.IsDeleted = true;
            comment.UpdatedAt = DateTime.UtcNow;

            _repository.Update(comment);
            return await _repository.SaveChangesAsync();
        }
    }
}