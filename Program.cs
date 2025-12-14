using CalculadoraImportacionesWeb.Data;
using CalculadoraImportacionesWeb.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar servicios Blazor
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

// 2. Configurar SQLite (igual que tu app original)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=productos.db"));

// 3. Configurar HttpClient para CotizacionService
builder.Services.AddHttpClient<ICotizacionService, CotizacionService>(client =>
{
    client.Timeout = TimeSpan.FromSeconds(15);
    client.DefaultRequestHeaders.Add("User-Agent", "CalculadoraImportacionesWeb");
});

// 4. Registrar tus servicios personalizados
builder.Services.AddScoped<ICalculadoraService, CalculadoraService>();

// 5. Configurar logging para ver errores
builder.Logging.AddConsole();

var app = builder.Build();

// Configurar pipeline HTTP
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

// Inicializar base de datos con datos de prueba
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    
    try
    {
        // Crear la base de datos si no existe
        dbContext.Database.EnsureCreated();
        
        // Agregar datos iniciales si la tabla está vacía
        if (!dbContext.Productos.Any())
        {
            dbContext.Productos.Add(new Models.Producto
            {
                Descripcion = "Producto de Ejemplo",
                ValorFOBYuan = 1000,
                PesoUnitario = 2.5m,
                Cantidad = 10,
                PorcentajeArancelAduana = 0.30m,
                PorcentajeArancelEstadisticas = 0.025m,
                IVA = 0.21m,
                FechaIngreso = DateTime.Now
            });
            dbContext.SaveChanges();
            Console.WriteLine("✅ Datos de prueba agregados");
        }
        
        Console.WriteLine("✅ Base de datos lista");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"⚠️ Error inicializando BD: {ex.Message}");
    }
}

app.Run();
