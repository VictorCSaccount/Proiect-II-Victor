using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
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
// Această linie forțează serverul să îți arate parola sau textul exact care provoacă eroarea
Microsoft.IdentityModel.Logging.IdentityModelEventSource.ShowPII = true;



// 1. Conexiunea la MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. OBLIGATORIU: Serviciile de Identity (Fără asta, Seeding-ul nu merge!)
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();


// 1. Înregistrează serviciul de token (uzina de bilet)
builder.Services.AddScoped<ITokenService, TokenService>();

// 2. Înregistrează serviciul de auth (logica de login/register)
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddScoped<IFoxRepository, FoxRepository>();
builder.Services.AddScoped<IAdoptionRepository, AdoptionRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IEnclosureRepository, EnclosureRepository>();

// Înregistrarea generică - syntaxa e specială pentru că avem <T>
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//serviciu  pentru automapper!!!


// se intregistreaza serviicul de email extern
builder.Services.AddScoped<IEmailService, EmailService>();



//builder.Services.AddAuthentication(options =>
//{
//    // Setăm JWT ca schemă implicită pentru a evita eroarea 500 (No authenticationScheme specified)
//    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
//    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
//})
//.AddJwtBearer(options =>
//{
//    options.TokenValidationParameters = new TokenValidationParameters
//    {
//        ValidateIssuer = true,
//        ValidateAudience = true,
//        ValidateLifetime = true,
//        ValidateIssuerSigningKey = true,
//        ValidIssuer = builder.Configuration["Jwt:Issuer"],
//        ValidAudience = builder.Configuration["Jwt:Audience"],
//        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!))
//    };

//    options.Events = new JwtBearerEvents
//    {
//        OnMessageReceived = context =>
//        {
//            context.Token = context.Request.Cookies["jwt_access_token"];
//            return Task.CompletedTask;
//        }
//    };
//});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Acestea sunt setările tale normale pentru validarea token-ului
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };


    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        // Extragem cookie-ul creat la login
    //        var token = context.Request.Cookies["jwt_access_token"];

    //        // Dacă există, îl dăm mai departe middleware-ului pentru validare
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            context.Token = token;
    //        }

    //        return Task.CompletedTask;
    //    }
    //};

    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        var token = context.Request.Cookies["jwt_access_token"];
    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            context.Token = token;
    //        }
    //        return Task.CompletedTask;
    //    },
    //    // AICI ESTE DETECTORUL DE MINCIUNI
    //    OnAuthenticationFailed = context =>
    //    {
    //        Console.WriteLine("\n==========================================");
    //        Console.WriteLine("🚨 MOTIVUL EXACT AL RESPINGERII JWT:");
    //        Console.WriteLine(context.Exception.Message);
    //        Console.WriteLine("==========================================\n");
    //        return Task.CompletedTask;
    //    }
    //};


    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        var token = context.Request.Cookies["jwt_access_token"];

    //        // 1. VEDEM EXACT CE E ÎN COOKIE ÎNAINTE DE CURĂȚARE
    //        Console.WriteLine($"\n==========================================");
    //        Console.WriteLine($"🚨 [DEBUG] Token brut scos din Cookie: '{token}'");
    //        Console.WriteLine($"==========================================\n");

    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            // 2. CURĂȚĂM GUNOIUL (eliminăm ghilimelele, spațiile sau prefixul Bearer)
    //            token = token.Replace("Bearer ", "").Trim('"').Trim();

    //            context.Token = token;
    //        }

    //        return Task.CompletedTask;
    //    },
    //    OnAuthenticationFailed = context =>
    //    {
    //        Console.WriteLine("\n🚨 MOTIVUL RESPINGERII:");
    //        Console.WriteLine(context.Exception.Message);
    //        return Task.CompletedTask;
    //    }
    //};


    //options.Events = new JwtBearerEvents
    //{
    //    OnMessageReceived = context =>
    //    {
    //        // 1. Tăiem forțat orice Header invalid trimis din greșeală de Swagger
    //        context.Request.Headers.Remove("Authorization");

    //        // 2. Luăm biletul doar din seiful nostru
    //        var token = context.Request.Cookies["jwt_access_token"];

    //        if (!string.IsNullOrEmpty(token))
    //        {
    //            context.Token = token.Replace("Bearer ", "").Trim('"').Trim();
    //        }

    //        return Task.CompletedTask;
    //    },
    //    OnAuthenticationFailed = context =>
    //    {
    //        Console.WriteLine("\n🚨 MOTIVUL EXACT AL RESPINGERII:");
    //        Console.WriteLine(context.Exception.Message);
    //        return Task.CompletedTask;
    //    }
    //};
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            // Luăm biletul din Cookie
            var token = context.Request.Cookies["jwt_access_token"];

            Console.WriteLine($"\n[DEBUG] Ce a venit pe teava: '{token}'");

            // Filtrul Antiglonț: E null? E gol? Are cuvântul 'undefined'? Nu are puncte?
            if (!string.IsNullOrWhiteSpace(token) && token.Contains('.') && token != "undefined")
            {
                // Curățăm și îl dăm mai departe doar dacă trece testul
                context.Token = token.Replace("Bearer ", "").Trim('"').Trim();
                Console.WriteLine("[DEBUG] Token acceptat si trimis la validare.");
            }
            else
            {
                Console.WriteLine("[DEBUG] Token respins la vama (format invalid).");
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"EROARE DE VALIDARE: {context.Exception.Message}");
            return Task.CompletedTask;
        }
    };




});







builder.Services.AddAutoMapper(typeof(MappingProfile));


// 3. Servicii pentru API și Swagger (PĂSTREAZĂ-LE, sunt utile pentru testat)
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Proiect II API", Version = "v1" });

    // 1. Definim schema de securitate
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Introdu token-ul JWT (fără cuvântul Bearer în față)."
    });

    // 2. Cerem Swagger-ului să trimită token-ul la fiecare cerere
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// permite formward al pagina -- pentru atrece prin https
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = Microsoft.AspNetCore.HttpOverrides.ForwardedHeaders.XForwardedProto
});


//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<ApplicationDbContext>();

//        // Asta e piesa lipsă! Fără ea, tabelele sunt invizibile.
//        await context.Database.MigrateAsync();

//        var mapper = services.GetRequiredService<AutoMapper.IMapper>();
//        // mapper.ConfigurationProvider.AssertConfigurationIsValid(); // Comentează asta dacă tot crapă la pornire

//        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//        await DbInitializer.SeedData(context, userManager, roleManager);
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"eroare critica: {ex.Message}");
//        // Nu da throw aici dacă vrei ca aplicația să pornească măcar (pentru debug)
//    }
//}

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
            logger.LogInformation($"Incercare conectare la DB... (Rămase: {retries})");

            // 1. Aplicăm migrările (creăm tabelele)
            await context.Database.MigrateAsync();

            // 2. Rulăm seeding-ul
            await DbInitializer.SeedData(context, userManager, roleManager);

            dbReady = true;
            logger.LogInformation("Succes: Baza de date este populată!");
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning($"DB nu e gata încă. Eroare: {ex.Message}");
            if (retries == 0)
            {
                logger.LogCritical("Eșec total după 10 încercări.");
                throw;
            }
            await Task.Delay(5000); // Așteptăm 5 secunde înainte de reîncercare
        }
    }
}



// 5. Pipeline-ul HTTP
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Accesează /swagger în browser ca să vezi API-ul
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();  

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();