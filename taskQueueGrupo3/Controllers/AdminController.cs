using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace taskQueueGrupo3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public IActionResult ManageUsers()
        {
            var users = _userManager.Users.ToList();
            return View(users);
        }

        [HttpPost]
        public async Task<IActionResult> CreateUser(string email, string password)
        {
            var user = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                return RedirectToAction("ManageUsers");
            }
            return View("Error");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("ManageUsers");
        }

        [HttpPost]
        public async Task<IActionResult> ToggleAdminRole(string id, bool isAdmin)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (user == null)
                {
                    return NotFound();
                }

                    if (isAdmin)
                {
                    // Agregar rol de admin
                    var result = await _userManager.AddToRoleAsync(user, "Admin");
                }
                else
                {
                    // Quitar rol de admin
                    var result = await _userManager.RemoveFromRoleAsync(user, "Admin");
                }
            }
            return Ok();
        }
    }
}
