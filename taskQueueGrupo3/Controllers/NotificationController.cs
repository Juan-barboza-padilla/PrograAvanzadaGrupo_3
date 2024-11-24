using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;
using System.Threading.Tasks;
using System.Linq;

namespace taskQueueGrupo3.Controllers
{
    public class NotificationController : Controller
    {
        private readonly TaskContext _context;

        public NotificationController(TaskContext context)
        {
            _context = context;
        }

        // GET: Notification/Index
        public async Task<IActionResult> Index()
        {
            var notifications = await _context.Set<Notification>().ToListAsync();
            return View(notifications);
        }

        // GET: Notification/History
        public async Task<IActionResult> History()
        {
            var notifications = await _context.Set<Notification>().ToListAsync();
            return View(notifications);
        }


        // POST: Notification/Send
        [HttpPost]
        public async Task<IActionResult> Send(int taskId)
        {
            var task = await _context.Tasks.FindAsync(taskId);
            if (task == null)
            {
                return NotFound();
            }

            var notification = new Notification
            {
                TaskId = taskId,
                RecipientEmail = "user@example.com", // Lógica para obtener correo del usuario
                Message = "Su tarea ha sido actualizada.",
                SentDate = DateTime.Now,
                IsSuccess = true
            };

            _context.Add(notification);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}