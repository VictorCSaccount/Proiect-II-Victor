using Microsoft.EntityFrameworkCore;
using ProiectII.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Configurează conexiunea la MySQL
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// 2. ADĂUGAT: Servicii necesare pentru Swagger/API
builder.Services.AddControllersWithViews();
builder.Services.AddEndpointsApiExplorer(); // <--- LINIE NOUĂ
builder.Services.AddSwaggerGen();          // <--- LINIE NOUĂ

var app = builder.Build();

// 3. Configurează pipeline-ul HTTP
// Swagger se activează de obicei doar în Development
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   // <--- LINIE NOUĂ: Generarea fișierului JSON
    app.UseSwaggerUI();  // <--- LINIE NOUĂ: Interfața vizuală
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

// 4. ADĂUGAT: Mapare pentru API Controllers
app.MapControllers(); // <--- LINIE NOUĂ: Foarte importantă pentru [ApiController]

// Rămâne și ruta default pentru MVC
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();