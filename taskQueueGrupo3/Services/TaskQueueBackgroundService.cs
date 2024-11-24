using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using taskQueueGrupo3.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading;
using System.Threading.Tasks;
using TaskModel = taskQueueGrupo3.Models.Task;
using Task = System.Threading.Tasks.Task;  // Evitar conflicto con System.Threading.Tasks

public class TaskQueueBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _delayBetweenTasks = TimeSpan.FromSeconds(30);

    public TaskQueueBackgroundService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var _context = scope.ServiceProvider.GetRequiredService<TaskContext>();

                var task = await _context.Tasks
                    .Where(t => t.Status == "Pendiente")
                    .OrderBy(t => t.Priority)
                    .ThenBy(t => t.ExecutionDate)
                    .FirstOrDefaultAsync(stoppingToken);

                if (task != null)
                {
                    task.Status = "En Proceso";
                    _context.Update(task);
                    await _context.SaveChangesAsync(stoppingToken);

                    bool success = await ProcessTask(task);

                    task.Status = success ? "Finalizada" : "Fallida";
                    _context.Update(task);
                    await _context.SaveChangesAsync(stoppingToken);

                    await CreateNotification(_context, task, success);

                    await Task.Delay(_delayBetweenTasks, stoppingToken);  // Usa Task.Delay para no bloquear
                }
                else
                {
                    await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
                }
            }
        }
    }

    private async Task<bool> ProcessTask(TaskModel task)
    {
        try
        {
            // Implementación de la lógica real de la tarea
            Console.WriteLine($"Ejecutando tarea: {task.Name}");
            await Task.Delay(TimeSpan.FromSeconds(10));  // Ajusta según la lógica real

            return true;  // Suponiendo éxito; ajusta según sea necesario
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error en la ejecución de la tarea '{task.Name}': {ex.Message}");
            return false;
        }
    }

    private async Task CreateNotification(TaskContext context, TaskModel task, bool success)
    {
        var notification = new Notification
        {
            TaskId = task.Id,
            RecipientEmail = "user@example.com",  // Obtener el correo del usuario real asociado a la tarea
            Message = success ? $"La tarea '{task.Name}' se completó con éxito." : $"La tarea '{task.Name}' falló.",
            SentDate = DateTime.Now,
            IsSuccess = success
        };

        context.Notifications.Add(notification);
        await context.SaveChangesAsync();
    }
}
