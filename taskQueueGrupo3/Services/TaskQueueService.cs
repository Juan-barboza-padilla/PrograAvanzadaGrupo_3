using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using taskQueueGrupo3.Models;
using TaskModel = taskQueueGrupo3.Models.Task;
using System.Security.Claims;

namespace taskQueueGrupo3.Services
{
    public class TaskQueueService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<TaskQueueService> _logger;
        private readonly IEmailService _emailService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public TaskQueueService(IServiceProvider serviceProvider, ILogger<TaskQueueService> logger, IEmailService emailService, IHttpContextAccessor httpContextAccessor)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _emailService = emailService;
            _httpContextAccessor = httpContextAccessor;
        }

        protected override async System.Threading.Tasks.Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("TaskQueueService: El servicio de tareas está ejecutándose correctamente.");

            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<TaskContext>();
                _logger.LogInformation("Contexto de base de datos inicializado correctamente.");

                // Obtener el correo electrónico del usuario logueado
                var userEmail = "davidjuanca29@gmail.com";
                if (string.IsNullOrEmpty(userEmail))
                {
                    _logger.LogError("No se pudo obtener el correo electrónico del usuario logueado.");
                    return;
                }

                _logger.LogInformation($"Correo del usuario logueado: {userEmail}");

                while (!stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation("TaskQueueService is running.");

                    var tasks = await context.Tasks
                        .Where(t => t.Status == "Pendiente")
                        .OrderBy(t => t.Priority)
                        .ThenBy(t => t.ExecutionDate)
                        .ToListAsync();

                    _logger.LogInformation($"Tareas pendientes encontradas: {tasks.Count}");

                    if (!tasks.Any())
                    {
                        _logger.LogWarning("No se encontraron tareas pendientes para procesar.");
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                        continue;
                    }

                    foreach (var task in tasks)
                    {
                        _logger.LogInformation($"Processing task: {task.Name}");

                        try
                        {
                            task.Status = "En Proceso";
                            context.Update(task);
                            var saveResult = await context.SaveChangesAsync();
                            _logger.LogInformation($"Actualización de tarea guardada. Resultado: {saveResult}");

                            bool success = await ProcessTask(context, task);
                            task.Status = success ? "Finalizada" : "Fallida";
                            context.Update(task);
                            await context.SaveChangesAsync();
                            _logger.LogInformation($"Task {task.Name} status updated to {task.Status}");

                            await NotifyUser(context, task, success, userEmail);
                            await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(30), stoppingToken);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Error procesando la tarea {task.Name}: {ex.Message}");
                        }
                    }

                    await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10), stoppingToken);
                }
            }

            _logger.LogInformation("TaskQueueService is stopping.");
        }

        private async System.Threading.Tasks.Task NotifyUser(TaskContext context, TaskModel task, bool success, string userEmail)
        {
            var notification = new Notification
            {
                TaskId = task.Id,
                RecipientEmail = userEmail,
                Message = success ? $"La tarea '{task.Name}' se completó exitosamente." : $"La tarea '{task.Name}' falló.",
                SentDate = DateTime.Now,
                IsSuccess = success
            };

            context.Notifications.Add(notification);
            await context.SaveChangesAsync();
            _logger.LogInformation($"Notification for task {task.Name} added to database");

            try
            {
                await _emailService.SendEmailAsync(notification.RecipientEmail, "Notificación de Tarea", notification.Message);
                _logger.LogInformation($"Correo enviado a {notification.RecipientEmail} para la tarea {task.Name}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error enviando correo para la tarea {task.Name}: {ex.Message}");
            }
        }

        private async System.Threading.Tasks.Task<bool> ProcessTask(TaskContext context, TaskModel task)
        {
            try
            {
                _logger.LogInformation($"Procesando tarea: {task.Name}");
                await System.Threading.Tasks.Task.Delay(TimeSpan.FromSeconds(10));

                var log = new TaskLog
                {
                    TaskId = task.Id,
                    LogMessage = $"Tarea '{task.Name}' ejecutada exitosamente.",
                    LogDate = DateTime.Now
                };

                context.TaskLogs.Add(log);
                await context.SaveChangesAsync();
                _logger.LogInformation($"Log de tarea '{task.Name}' registrado correctamente.");

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error procesando tarea '{task.Name}': {ex.Message}");

                var log = new TaskLog
                {
                    TaskId = task.Id,
                    LogMessage = $"Error procesando tarea: {ex.Message}",
                    LogDate = DateTime.Now
                };

                context.TaskLogs.Add(log);
                await context.SaveChangesAsync();

                return false;
            }
        }
    }
}
