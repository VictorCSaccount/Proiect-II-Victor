
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using ProiectII.DTO;
using ProiectII.DTO.AuthAccount;
using ProiectII.Interfaces;
using ProiectII.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;




namespace ProiectII.Services.SecurityIdentity
{


   
        public class AuthService : IAuthService
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            private readonly IConfiguration _configuration;

            public AuthService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IConfiguration configuration)
            {
                _userManager = userManager;
                _roleManager = roleManager;
                _configuration = configuration; // Ai nevoie de asta ca să citești cheia secretă din appsettings.json
            }

            public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
            {
                // 1. Verificăm dacă există email-ul
                var user = await _userManager.FindByEmailAsync(dto.Email);
                if (user == null) return null;

                // 2. CRITIC: Verificăm dacă contul a fost banat/dezactivat (regula din diagrama ta)
                if (!user.IsActive)
                    throw new UnauthorizedAccessException($"Cont dezactivat. Motiv: {user.DeactivationReason}");

                // 3. Verificăm parola
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, dto.Password);
                if (!isPasswordValid) return null;

                // 4. Updatăm data ultimei logări
                user.UpdateLastLogin();
                await _userManager.UpdateAsync(user);

                // 5. Generăm token-ul
                var token = await GenerateJwtToken(user);

                // 6. Returnăm DTO-ul conform diagramelor tale
                var roles = await _userManager.GetRolesAsync(user);
                return new AuthResponseDto
                {
                    Token = token,
                    Expiration = DateTime.UtcNow.AddHours(2),
                    UserRole = roles.FirstOrDefault() ?? "User",
                    RefreshToken = "NOT_IMPLEMENTED_YET" // Deocamdată îl lăsăm așa
                };
            }

            public async Task<MessageDto> RegisterAsync(RegisterDto dto, string role = "User")
            {
                var userExists = await _userManager.FindByEmailAsync(dto.Email);
                if (userExists != null)
                    return new MessageDto { IsSuccess = false, Message = "Email-ul este deja folosit." };

                // Aici nu folosim AutoMapper pentru că parola are nevoie de Hashing, nu de o simplă copiere
                var newUser = new ApplicationUser
                {
                    UserName = dto.Username,
                    Email = dto.Email,
                    FirstName = dto.FirstName,
                    LastName = dto.LastName,
                    BornDate = dto.BirthDate,
                    IsActive = true
                };

                var result = await _userManager.CreateAsync(newUser, dto.Password);

                if (!result.Succeeded)
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    return new MessageDto { IsSuccess = false, Message = $"Eroare la creare: {errors}" };
                }

                // Atribuim rolul (dacă rolul nu există, sistemul ar trebui să crape, dar presupunem că DBInitializer le-a creat)
                if (await _roleManager.RoleExistsAsync(role))
                {
                    await _userManager.AddToRoleAsync(newUser, role);
                }

                return new MessageDto { IsSuccess = true, Message = "Cont creat cu succes!" };
            }


        // verificare login
        public async Task<bool> IsUserActiveAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            return user != null && user.IsActive;
        }

        // ==================================================
        // Generare cheie (JWT)
        // ==================================================
        private async Task<string> GenerateJwtToken(ApplicationUser user)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id), // Id-ul din DB (String)
                new Claim(ClaimTypes.Email, user.Email!),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

                // Adăugăm rolurile în Token
                foreach (var userRole in userRoles)
                {
                    authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                }

            var secret = _configuration["Jwt:Secret"];
                var issuer = _configuration["Jwt:Issuer"];
                var audience = _configuration["Jwt:Audience"];

                var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret!));

                var token = new JwtSecurityToken(
                    issuer: issuer,
                    audience: audience,
                    expires: DateTime.UtcNow.AddHours(2),
                    claims: authClaims,
                    signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

                return new JwtSecurityTokenHandler().WriteToken(token);
            }
        }
    }

