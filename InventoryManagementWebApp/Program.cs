using InventoryManagementWebApp.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// Conexi�n de la BBDD Config
builder.Services.AddDbContext<AppDBContext>(options =>
{
    // Cadena de Conexion SQL Server
    options.UseSqlServer(builder.Configuration.GetConnectionString("connection"));
});

// Autenticacion de Login Config
// code..

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

// Autenticaci�n ejecuci�n
// code..

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Acceso}/{action=Login}/{id?}");

app.Run();
