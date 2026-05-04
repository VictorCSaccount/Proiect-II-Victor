using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.HttpOverrides;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Mappings;
using ProiectII.Models;
using ProiectII.Repositories;
using ProiectII.Services.CoreDomain;
using ProiectII.Services.SecurityIdentity;
using ProiectII.Services.UtilityServices;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;

// ==========================================
// 1. BAZA DE DATE & IDENTITY
// ==========================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// ==========================================
// 2. DEPENDENCY INJECTION
// ==========================================
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IFoxRepository, FoxRepository>();
builder.Services.AddScoped<IAdoptionRepository, AdoptionRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IEnclosureRepository, EnclosureRepository>();
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IAdoptionService, AdoptionService>();
builder.Services.AddScoped<IFoxService, FoxService>();

builder.Services.AddAutoMapper(typeof(MappingProfile));

// ==========================================
// 3. AUTENTIFICARE JWT
// ==========================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.Request.Cookies["jwt_access_token"];
            Console.WriteLine($"\n[DEBUG] Cookie JWT: '{token}'");
            if (!string.IsNullOrWhiteSpace(token) && token.Contains('.') && token != "undefined")
            {
                context.Token = token.Replace("Bearer ", "").Trim('"').Trim();
                Console.WriteLine("[DEBUG] Token acceptat.");
            }
            else
            {
                Console.WriteLine("[DEBUG] Token respins.");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[EROARE JWT]: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

// ==========================================
// 4. CORS
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7033")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// ==========================================
// 5. SWAGGER & CONTROLLERS
// ==========================================
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Proiect II API", Version = "v1" });
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introdu token-ul JWT."
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

// ==========================================
// 6. MIGRARE AUTOMATA LA STARTUP (fara seed)
// ==========================================
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    var logger = services.GetRequiredService<ILogger<Program>>();
//    var context = services.GetRequiredService<ApplicationDbContext>();

//    int retries = 10;
//    while (retries > 0)
//    {
//        try
//        {
//            logger.LogInformation($"[DB] Incercare migrare... (Ramase: {retries})");
//            await context.Database.MigrateAsync();
//            logger.LogInformation("[DB] Migrare reusita! API pornit.");

//            using (var innerScope = app.Services.CreateScope())
//            {
//                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
//                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

//                foreach (var role in new[] { "Admin", "User", "Employee" })
//                    if (!await roleManager.RoleExistsAsync(role))
//                        await roleManager.CreateAsync(new IdentityRole(role));

//                if (await userManager.FindByEmailAsync("admin@fox.com") == null)
//                {
//                    var admin = new ApplicationUser
//                    {
//                        UserName = "admin@fox.com",
//                        Email = "admin@fox.com",
//                        FirstName = "Victor",
//                        LastName = "Admin",
//                        BornDate = new DateOnly(1995, 5, 20),
//                        EmailConfirmed = true,
//                        IsActive = true,
//                        LastLogin = DateTime.UtcNow
//                    };

//                    await userManager.CreateAsync(admin, "SecurePass123!");
//                    await userManager.AddToRoleAsync(admin, "Admin");
//                }

//                if (await userManager.FindByEmailAsync("user@fox.com") == null)
//                {
//                    var user = new ApplicationUser
//                    {
//                        UserName = "user@fox.com",
//                        Email = "user@fox.com",
//                        FirstName = "Ion",
//                        LastName = "Popescu",
//                        BornDate = new DateOnly(2000, 1, 1),
//                        EmailConfirmed = true,
//                        IsActive = true,
//                        LastLogin = DateTime.UtcNow
//                    };

//                    await userManager.CreateAsync(user, "UserPass123!");
//                    await userManager.AddToRoleAsync(user, "User");
//                }
//            }


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();

    int retries = 10;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("[DB] Încercare migrare... (Rămase: {Retries})", retries);

            // 1. Aplică structura tabelelor
            await context.Database.MigrateAsync();
            logger.LogInformation("[DB] Migrare reușită! Tabelele există.");

            // 2. Extrage managerii necesari pentru Identity
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            // 3. Apelează clasa ta externă (asigură-te că namespace-ul ProiectII.Data este importat sus cu 'using')
            await DbInitializer.SeedData(context, userManager, roleManager);
            logger.LogInformation("[DB] Seeding-ul datelor a fost finalizat cu succes.");

            break; // Ieșim din buclă dacă totul a decurs fără erori
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning("[DB] Migrare/Seeding eșuat: {Message}", ex.Message);

            if (retries == 0)
            {
                logger.LogCritical("[DB] Eșec total la migrare și inițializare!");
                throw;
            }
            await Task.Delay(3000); // Pauză înainte de următoarea încercare
        }
    }
}








//            break;
//        }
//        catch (Exception ex)
//        {
//            retries--;
//            logger.LogWarning($"[DB] Migrare esuata: {ex.Message}");
//            if (retries == 0)
//            {
//                logger.LogCritical("[DB] Esec total la migrare!");
//                throw;
//            }
//            await Task.Delay(3000);
//        }
//    }
//}

// ==========================================
// 7. PIPELINE
// ==========================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseStaticFiles();
app.UseRouting();
app.UseCors("FrontendPolicy");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Urls.Add("http://*:8080");

app.Run();