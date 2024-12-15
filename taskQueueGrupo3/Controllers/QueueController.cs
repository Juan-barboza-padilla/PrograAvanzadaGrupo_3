using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;

namespace taskQueueGrupo3.Controllers
{
    public class QueueController : Controller
    {
        private readonly TaskContext _context;

        public QueueController(TaskContext context)
        {
            _context = context;
        }

        // GET: Queue/Index
        public async Task<IActionResult> Index()
		{
			var tasks = await _context.Tasks
				.OrderBy(t => t.Priority) // Ordenar por prioridad
				.ThenBy(t => t.ExecutionDate)
				.ToListAsync();

			return View(tasks);
		}

        // GET: Queue/Monitor
        public async Task<IActionResult> Monitor()
        {
            var tasks = await _context.Tasks
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.ExecutionDate)
                .ToListAsync();

            return View(tasks);
        }

        // POST: Queue/Retry/5
        [HttpPost]
		public async Task<IActionResult> Retry(int id)
		{
			var task = await _context.Tasks.FindAsync(id);
			if (task != null && task.Status == "Fallida")
			{
				task.Status = "Pendiente";
				_context.Update(task);
				await _context.SaveChangesAsync();
			}
			return RedirectToAction("Index");
		}

		// GET: Queue/Status
		public async Task<IActionResult> Status()
		{
			var taskSummary = new
			{
				TotalPendientes = await _context.Tasks.CountAsync(t => t.Status == "Pendiente"),
				TotalEnProceso = await _context.Tasks.CountAsync(t => t.Status == "En Proceso"),
				TotalFinalizadas = await _context.Tasks.CountAsync(t => t.Status == "Finalizada"),
				TotalFallidas = await _context.Tasks.CountAsync(t => t.Status == "Fallida")
			};

			return Json(taskSummary); // Devolver el resumen como JSON
		}

        

    }
}
