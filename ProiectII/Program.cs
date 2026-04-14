using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProiectII.Data;
using ProiectII.Interfaces;
using ProiectII.Mappings;
using ProiectII.Models;
using ProiectII.Repositories;
using ProiectII.Services.CoreDomain;
using ProiectII.Services.UtilityServices;

var builder = WebApplication.CreateBuilder(args);

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



builder.Services.AddScoped<IFoxRepository, FoxRepository>();
builder.Services.AddScoped<IAdoptionRepository, AdoptionRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IEnclosureRepository, EnclosureRepository>();

// Înregistrarea generică - syntaxa e specială pentru că avem <T>
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
//serviciu  pentru automapper!!!



builder.Services.AddAutoMapper(typeof(MappingProfile));


// 3. Servicii pentru API și Swagger (PĂSTREAZĂ-LE, sunt utile pentru testat)
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddScoped<IFileStorageService, FileStorageService>();
builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IReportRepository, ReportRepository>();
builder.Services.AddScoped<IReportService, ReportService>();

var app = builder.Build();

// 4. INTEGRARE SEEDING (Rulează o singură dată la pornire)
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    try
//    {
//        var context = services.GetRequiredService<ApplicationDbContext>();
//        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
//        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//        // Apelăm DbInitializer asincron
//        await DbInitializer.SeedData(context, userManager, roleManager);
//    }
//    catch (Exception ex)
//    {
//        var logger = services.GetRequiredService<ILogger<Program>>();
//        logger.LogError(ex, "A apărut o eroare la popularea bazei de date.");
//    }
//}


// 4. INTEGRARE SEEDING + VERIFICARE MAPPING
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // --- ADAUGĂ ASTA PENTRU AUTOMAPPER ---
        var mapper = services.GetRequiredService<AutoMapper.IMapper>();
        mapper.ConfigurationProvider.AssertConfigurationIsValid();
        // --------------------------------------

        var context = services.GetRequiredService<ApplicationDbContext>();
        var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

        await DbInitializer.SeedData(context, userManager, roleManager);
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        // Dacă eroarea e de la AutoMapper, aici vei vedea detaliile
        logger.LogError(ex, "Eroare critică la pornire (Mapping sau Seeding).");
        throw; // Forțăm oprirea dacă maparea e greșită
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

// OBLIGATORIU: Ordinea contează!
app.UseAuthentication(); // Cine ești?
app.UseAuthorization();  // Ce ai voie să faci?

app.MapControllers();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();