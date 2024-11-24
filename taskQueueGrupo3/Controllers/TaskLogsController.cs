using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;

namespace taskQueueGrupo3.Controllers
{
    [Authorize]
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
            var taskContext = _context.TaskLogs.Include(t => t.Task);
            return View(await taskContext.ToListAsync());
        }

        // GET: TaskLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLog = await _context.TaskLogs
                .Include(t => t.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskLog == null)
            {
                return NotFound();
            }

            return View(taskLog);
        }

        // GET: TaskLogs/Create
        public IActionResult Create()
        {
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Id");
            return View();
        }

        // GET: TaskLogs/LogDetails
        public async Task<IActionResult> LogDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLog = await _context.TaskLogs
                .Include(t => t.Task)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (taskLog == null)
            {
                return NotFound();
            }

            return View(taskLog);
        }


        // POST: TaskLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,TaskId,LogMessage,LogDate")] TaskLog taskLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(taskLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Id", taskLog.TaskId);
            return View(taskLog);
        }

        // GET: TaskLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLog = await _context.TaskLogs.FindAsync(id);
            if (taskLog == null)
            {
                return NotFound();
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Id", taskLog.TaskId);
            return View(taskLog);
        }

        // POST: TaskLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,TaskId,LogMessage,LogDate")] TaskLog taskLog)
        {
            if (id != taskLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(taskLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TaskLogExists(taskLog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["TaskId"] = new SelectList(_context.Tasks, "Id", "Id", taskLog.TaskId);
            return View(taskLog);
        }

        // GET: TaskLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var taskLog = await _context.TaskLogs
                .Include(t => t.Task)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (taskLog == null)
            {
                return NotFound();
            }

            return View(taskLog);
        }

        // POST: TaskLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var taskLog = await _context.TaskLogs.FindAsync(id);
            if (taskLog != null)
            {
                _context.TaskLogs.Remove(taskLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TaskLogExists(int id)
        {
            return _context.TaskLogs.Any(e => e.Id == id);
        }
    }
}
