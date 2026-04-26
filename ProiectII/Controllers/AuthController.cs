using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO;
using ProiectII.DTO.AuthAccount;
using ProiectII.Interfaces;
using ProiectII.Models;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            // 1. Apelăm serviciul pentru validare și generare Token
            var authResponse = await _authService.LoginAsync(dto);

            if (authResponse == null)
            {
                return Unauthorized(new { Message = "Email sau parolă incorecte, sau cont inactiv." });
            }

            // 2. Configurare Cookie de SESIUNE (fără proprietatea Expires)
            // Acest cookie va fi șters automat de browser când acesta este închis.
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,         // Protecție XSS: JavaScript nu poate citi token-ul
                Secure = true,           // Trimis doar prin HTTPS
                SameSite = SameSiteMode.Strict, // Protecție CSRF: Cookie-ul nu pleacă spre alte site-uri
                Path = "/"               // Disponibil pentru tot API-ul
            };

            // 3. Atașăm token-ul la răspunsul HTTP
            Response.Cookies.Append("jwt_access_token", authResponse.Token, cookieOptions);

            // 4. Returnăm datele utilizatorului (fără a forța frontend-ul să stocheze token-ul manual)
            return Ok(new
            {
                authResponse.UserRole,
                authResponse.Expiration,
                Message = "Logat cu succes."
            });
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            // Înregistrăm utilizatorul cu rolul implicit "User"
            var result = await _authService.RegisterAsync(dto, "User");

            if (!result.IsSuccess)
            {
                return BadRequest(new { Message = result.Message });
            }

            return Ok(new { Message = result.Message });
        }


        
        [HttpGet("check-auth")]
        [Microsoft.AspNetCore.Authorization.Authorize]
        public IActionResult CheckAuth()
        {
            // Dacă request-ul trece de [Authorize], Cookie-ul este automat valid
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

            return Ok(new
            {
                IsAuthenticated = true,
                UserId = userId,
                Role = userRole
            });
        }



        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // Pentru a șterge un cookie, trebuie să îi spunem browserului să îl expire imediat.
            // Este important să folosim aceleași opțiuni (HttpOnly, Secure) ca la creare.
            Response.Cookies.Delete("jwt_access_token", new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Path = "/"
            });

            return Ok(new { Message = "Delogat cu succes. Sesiunea a fost închisă." });
        }



        public AuthController(UserManager<ApplicationUser> userManager, IEmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);

            if (user == null)
            {
                return Ok(new { message = "Instrucțiunile au fost trimise." });
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            await _emailService.SendEmailAsync(
                user.Email,
                "Resetare Parolă",
                $"Codul tău de securitate este: {token}"
            );

            return Ok(new { message = "Instrucțiunile au fost trimise." });
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            // 1. Căutăm utilizatorul după email
            var user = await _userManager.FindByEmailAsync(dto.Email);

            // Regula de securitate: Nu dăm detalii specifice dacă userul nu există, 
            // dar aici, fiind un pas de resetare, returnăm eroare generică de cerere invalidă.
            if (user == null)
            {
                return BadRequest(new { message = "Cerere de resetare invalidă." });
            }

            // 2. Utilizăm UserManager pentru a valida token-ul și a schimba parola.
            // ResetPasswordAsync face automat:
            // - Validarea token-ului (dacă a expirat sau dacă a fost deja folosit)
            // - Hash-uirea noii parole folosind algoritmul securizat (PBKDF2)
            // - Salvarea în baza de date
            var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);

            if (!result.Succeeded)
            {
                // Extragem descrierile erorilor (ex: parola prea scurtă, token invalid etc.)
                var errors = result.Errors.Select(e => e.Description);
                return BadRequest(new { message = "Eroare la resetarea parolei.", errors });
            }

            return Ok(new { message = "Parola a fost actualizată cu succes. Te poți loga." });
        }


    }
}