using Microsoft.AspNetCore.Identity;

namespace taskQueueGrupo3.Services
{
    public static class DataSeeder
    {
        public static async Task SeedAdminUserAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string adminRoleName = "Admin";
            string adminUserEmail = "admin@example.com";
            string adminPassword = "Admin123!";

            // Crear el rol si no existe
            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            // Crear el usuario administrador si no existe
            var adminUser = await userManager.FindByEmailAsync(adminUserEmail);
            if (adminUser == null)
            {
                var user = new IdentityUser
                {
                    UserName = adminUserEmail,
                    Email = adminUserEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, adminRoleName);
                    Console.WriteLine("Usuario administrador creado exitosamente.");
                }
                else
                {
                    Console.WriteLine("Error al crear el usuario administrador: " + result.Errors.FirstOrDefault()?.Description);
                }
            }
            else
            {
                Console.WriteLine("El usuario administrador ya existe.");
            }
        }
    }
}
