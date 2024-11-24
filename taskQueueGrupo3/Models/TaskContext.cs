using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using taskQueueGrupo3.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

public class TaskContext : IdentityDbContext<IdentityUser, IdentityRole, string> // Hereda de IdentityDbContext
{
    public DbSet<taskQueueGrupo3.Models.Task> Tasks { get; set; }
    public DbSet<TaskLog> TaskLogs { get; set; }
    public DbSet<Notification> Notifications { get; set; }


    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<taskQueueGrupo3.Models.Task>().ToTable("Tasks");
        modelBuilder.Entity<TaskLog>().ToTable("TaskLogs");
	}
}
