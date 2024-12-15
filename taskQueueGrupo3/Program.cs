using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;
using taskQueueGrupo3.Services;

var builder = WebApplication.CreateBuilder(args);

// Configurar logging para habilitar niveles detallados
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

// Agregar cadena de conexi�n al archivo appsettings.json
builder.Services.AddDbContext<TaskContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("TaskQueueGrupo3DBConnection")));

// Configurar Identity para la autenticaci�n y autorizaci�n
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<TaskContext>()
    .AddDefaultTokenProviders();

// Configuraci�n de las opciones de identidad (por ejemplo, contrase�as, bloqueos, etc.)
builder.Services.Configure<IdentityOptions>(options =>
{
    // Configuraci�n de la pol�tica de contrase�as
    options.Password.RequireDigit = true;
    options.Password.RequiredLength = 6;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = false;
    // Configuraci�n de bloqueo de cuenta
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 3;
    // Configuraci�n de usuarios
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = true;
});

// Agregar servicios de autenticaci�n
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

// Servicio de env�os SMTP
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddHttpContextAccessor();

// Construir la aplicaci�n
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

// Configurar la tuber�a de solicitud HTTP
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();

// Usar autenticaci�n y autorizaci�n
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
