using Microsoft.AspNetCore.Mvc;
using taskQueueGrupo3.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using TaskModel = taskQueueGrupo3.Models.Task;
using Task = System.Threading.Tasks.Task;

namespace taskQueueGrupo3.Controllers
{
    public class TaskExecutionController : Controller
    {
        private readonly TaskContext _context;

        public TaskExecutionController(TaskContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> StartQueueProcessing()
        {
            var tasks = await _context.Tasks
                .Where(t => t.Status == "Pendiente")
                .OrderBy(t => t.Priority)
                .ThenBy(t => t.ExecutionDate)
                .ToListAsync();

            foreach (var task in tasks)
            {
                task.Status = "En Proceso";
                _context.Update(task);
                await _context.SaveChangesAsync();

                bool success = await ProcessTask(task);

                task.Status = success ? "Finalizada" : "Fallida";
                _context.Update(task);
                await _context.SaveChangesAsync();

                await NotifyUser(task, success);
                await Task.Delay(TimeSpan.FromSeconds(30));
            }

            return Ok("Procesamiento de la cola completado");
        }

        private async Task NotifyUser(TaskModel task, bool success)
        {
            var notification = new Notification
            {
                TaskId = task.Id,
                RecipientEmail = "user@example.com",
                Message = success ? $"La tarea '{task.Name}' se completó exitosamente." : $"La tarea '{task.Name}' falló.",
                SentDate = DateTime.Now,
                IsSuccess = success
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();
        }

        private async Task<bool> ProcessTask(TaskModel task)
        {
            try
            {
                Console.WriteLine($"Ejecutando tarea: {task.Name}");
                await Task.Delay(TimeSpan.FromSeconds(10));

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la ejecución de la tarea '{task.Name}': {ex.Message}");
                return false;
            }
        }
    }
}
