    using global::ProiectII.DTO.AuthAccount;
    using ProiectII.DTO;

    namespace ProiectII.Interfaces
    {
        public interface IAuthService
        {
            // Returnează token-ul JWT dacă succes, sau aruncă o excepție/returnează null dacă eșuează
            Task<AuthResponseDto?> LoginAsync(LoginDto dto);

            // Înregistrează un user nou (angajații vor fi înregistrați de Admin, publicul se înregistrează singur)
            Task<MessageDto> RegisterAsync(RegisterDto dto, string role = "User");
        }
    }

