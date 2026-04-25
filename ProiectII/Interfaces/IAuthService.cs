    using global::ProiectII.DTO.AuthAccount;
    using ProiectII.DTO;

    namespace ProiectII.Interfaces
    {
        public interface IAuthService
        {
            Task<AuthResponseDto?> LoginAsync(LoginDto dto);

            Task<MessageDto> RegisterAsync(RegisterDto dto, string role = "User");
             Task<bool> IsUserActiveAsync(string userId);
         }
    }

