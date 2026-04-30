using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
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
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Afiseaza erorile detaliate pentru Identity
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
// 2. DEPENDENCY INJECTION (Servicii & Repo)
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

builder.Services.AddAutoMapper(typeof(MappingProfile));

// ==========================================
// 3. AUTENTIFICARE JWT (Cu extragere din Cookie)
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
            Console.WriteLine($"\n[DEBUG] Ce a venit pe teava din Cookie: '{token}'");

            if (!string.IsNullOrWhiteSpace(token) && token.Contains('.') && token != "undefined")
            {
                context.Token = token.Replace("Bearer ", "").Trim('"').Trim();
                Console.WriteLine("[DEBUG] Token acceptat si trimis la validare.");
            }
            else
            {
                Console.WriteLine("[DEBUG] Token respins la vama (format invalid sau lipsa).");
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[EROARE DE VALIDARE JWT]: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };
});

// ==========================================
// 4. CONFIGURARE CORS PENTRU FRONTEND
// ==========================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontendPolicy", policy =>
    {
        policy.WithOrigins("https://localhost:7033")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials(); // Critic pentru a permite Cookie-urile intre porturi
    });
});

// ==========================================
// 5. SWAGGER & API CONTROLLERS
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
        Description = "Introdu token-ul JWT (fără cuvântul Bearer în față)."
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

// ==========================================
// ==========================================
// BUILDER END - INCEPE CONFIGURAREA PIPELINE-ULUI
// ==========================================
// ==========================================

var app = builder.Build();

// Permite Forward Headers pentru Nginx (HTTPS Termination)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

// ==========================================
// 6. INITIALIZARE BAZA DE DATE (Seeding & Migrare)
// ==========================================
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var context = services.GetRequiredService<ApplicationDbContext>();
    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

    int retries = 10;
    bool dbReady = false;

    while (!dbReady && retries > 0)
    {
        try
        {
            logger.LogInformation($"Incercare conectare la DB... (Ramase: {retries})");
            //await context.Database.MigrateAsync();
            await DbInitializer.SeedData(context, userManager, roleManager);
            dbReady = true;
            logger.LogInformation("Succes: Baza de date este populata!");
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning($"DB nu e gata inca. Eroare: {ex.Message}");
            if (retries == 0)
            {
                logger.LogCritical("Esec total dupa 10 incercari.");
                throw;
            }
            await Task.Delay(5000);
        }
    }
}

// ==========================================
// 7. HTTP REQUEST PIPELINE (Ordinea este matematica)
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

// CORS trebuie sa fie exact aici, intre Routing si Authentication
app.UseCors("FrontendPolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Fortare port intern Docker pentru comunicarea cu Nginx
app.Urls.Add("http://*:8080");

app.Run();