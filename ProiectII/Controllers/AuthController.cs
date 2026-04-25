using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO;
using ProiectII.DTO.AuthAccount;
using ProiectII.Interfaces;

namespace ProiectII.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

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
    }
}