using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;
using System.Threading.Tasks;

public class TaskLogsController : Controller
{
    private readonly TaskContext _context;

    public TaskLogsController(TaskContext context)
    {
        _context = context;
    }

    // GET: TaskLogs
    public async Task<IActionResult> Index()
    {
        var taskLogs = await _context.TaskLogs.Include(t => t.Task).ToListAsync();
        return View(taskLogs);
    }
}