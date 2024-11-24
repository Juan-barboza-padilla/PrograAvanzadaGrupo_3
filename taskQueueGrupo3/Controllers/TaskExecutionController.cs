using Microsoft.AspNetCore.Mvc;
using taskQueueGrupo3.Models;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Linq;
using TaskModel = taskQueueGrupo3.Models.Task;
using Task = System.Threading.Tasks.Task;  // Evitar conflicto con System.Threading.Tasks

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
                await Task.Delay(TimeSpan.FromSeconds(30));  // Usa System.Threading.Tasks.Task
            }

            return Ok("Procesamiento de la cola completado");
        }

        private async Task NotifyUser(TaskModel task, bool success)
        {
            var notification = new Notification
            {
                TaskId = task.Id,
                RecipientEmail = "user@example.com",  // Debe enlazarse al usuario real
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
                // Implementación real de la lógica de la tarea
                Console.WriteLine($"Ejecutando tarea: {task.Name}");
                await Task.Delay(TimeSpan.FromSeconds(10));  // Ajusta la duración según sea necesario

                return true;  // Ajusta según la lógica específica
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error en la ejecución de la tarea '{task.Name}': {ex.Message}");
                return false;
            }
        }
    }
}
