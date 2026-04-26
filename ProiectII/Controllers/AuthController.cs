using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ProiectII.DTO.AuthAccount;
using ProiectII.Interfaces;
using ProiectII.Models;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;

    // SINGURUL constructor permis - aici mufăm toate dependențele
    public AuthController(
        IAuthService authService,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService)
    {
        _authService = authService;
        _userManager = userManager;
        _emailService = emailService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var authResponse = await _authService.LoginAsync(dto);
        if (authResponse == null) return Unauthorized(new { Message = "Date incorecte." });

        var cookieOptions = new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        };

        Response.Cookies.Append("jwt_access_token", authResponse.Token, cookieOptions);
        return Ok(new { authResponse.UserRole, Message = "Logat cu succes." });
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _authService.RegisterAsync(dto, "User");
        if (!result.IsSuccess) return BadRequest(new { Message = result.Message });
        return Ok(new { Message = result.Message });
    }

    [HttpGet("check-auth")]
    [Authorize]
    public IActionResult CheckAuth()
    {
        return Ok(new
        {
            IsAuthenticated = true,
            UserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
            Role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
        });
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("jwt_access_token", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Strict,
            Path = "/"
        });
        return Ok(new { Message = "Delogat." });
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto dto)
    {
        // Acum _userManager nu mai este null, deci nu va mai crăpa
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return Ok(new { message = "Instrucțiunile au fost trimise." });

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
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user == null) return BadRequest(new { message = "Cerere invalidă." });

        var result = await _userManager.ResetPasswordAsync(user, dto.Token, dto.NewPassword);
        if (!result.Succeeded)
        {
            return BadRequest(new { message = "Eroare la resetare.", errors = result.Errors.Select(e => e.Description) });
        }

        return Ok(new { message = "Parola a fost actualizată." });
    }
}