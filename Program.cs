using CalculadoraImportacionesWeb.Data;
using CalculadoraImportacionesWeb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios de Razor Pages y Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 2. Configurar base de datos SQLite (igual que tu aplicación original)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=productos.db"));

// 3. Configurar HttpClient para servicios externos (API de cotizaciones)
builder.Services.AddHttpClient<ICotizacionService, CotizacionService>();

// 4. Registrar servicios personalizados
builder.Services.AddScoped<ICalculadoraService, CalculadoraService>();

// 5. Configurar para que los porcentajes usen punto decimal (invariante a la cultura)
AppContext.SetSwitch("System.Globalization.Invariant", true);

var app = builder.Build();

// Configurar el pipeline HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

// 6. Asegurar que la base de datos esté creada y con datos iniciales
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        // Crear la base de datos si no existe
        dbContext.Database.EnsureCreated();
        
        // Verificar si ya existen ajustes
        if (!dbContext.Ajustes.Any())
        {
            Console.WriteLine("✅ Creando datos iniciales en la base de datos...");
            
            // Los datos iniciales ya están configurados en AppDbContext.OnModelCreating
            // Entity Framework los agregará automáticamente
            dbContext.SaveChanges();
        }
        
        // Verificar si hay productos de ejemplo (opcional)
        if (!dbContext.Productos.Any())
        {
            Console.WriteLine("⚠️ No hay productos en la base de datos.");
            Console.WriteLine("   Agrega productos desde la interfaz web.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error al inicializar la base de datos: {ex.Message}");
    }
}

Console.WriteLine("🚀 Aplicación lista en: https://localhost:5001");
Console.WriteLine("   o http://localhost:5000");

app.Run();
