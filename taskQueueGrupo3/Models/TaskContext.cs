using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using taskQueueGrupo3.Models;
using TaskModel = taskQueueGrupo3.Models.Task;

public class TaskContext : IdentityDbContext<IdentityUser, IdentityRole, string>
{
    public DbSet<TaskModel> Tasks { get; set; }
    public DbSet<TaskLog> TaskLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }

    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<TaskModel>().ToTable("Tasks");
        modelBuilder.Entity<TaskLog>().ToTable("TaskLogs");
    }
}