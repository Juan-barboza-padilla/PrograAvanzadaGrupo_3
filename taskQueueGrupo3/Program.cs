using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;
using taskQueueGrupo3.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para habilitar niveles detallados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Agregar cadena de conexión al archivo appsettings.json
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskQueueGrupo3DBConnection")));

// Configurar Identity para la autenticación y autorización
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskContext>()
    .AddDefaultTokenProviders();

// Configuración de las opciones de identidad (por ejemplo, contraseñas, bloqueos, etc.)
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configuración de la política de contraseñas
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    // Configuración de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    // Configuración de usuarios
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

// Agregar servicios de autenticación
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Home/AccessDenied";
    });

// Agregar servicios al contenedor
builder.Services.AddControllersWithViews();

// Registrar el servicio TaskQueueService
builder.Services.AddHostedService<TaskQueueService>();

// Servicio de envíos SMTP
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

// Construir la aplicación
var app = builder.Build();

// Ejecutar el seeding del usuario administrador 
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        await DataSeeder.SeedAdminUserAsync(services);
        app.Logger.LogInformation("Usuario administrador inicial creado exitosamente.");
    }
    catch (Exception ex)
    {
        app.Logger.LogError($"Error al crear el usuario administrador: {ex.Message}");
    }
}

// Configurar la tubería de solicitud HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Usar autenticación y autorización
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
