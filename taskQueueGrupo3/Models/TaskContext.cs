using Microsoft.EntityFrameworkCore;
using taskQueueGrupo3.Models;

public class TaskContext : DbContext
{
    public DbSet<taskQueueGrupo3.Models.Task> Tasks { get; set; }
    public DbSet<TaskLog> TaskLogs { get; set; }

    public TaskContext(DbContextOptions<TaskContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.Entity<taskQueueGrupo3.Models.Task>().ToTable("Tasks");
        modelBuilder.Entity<TaskLog>().ToTable("TaskLogs");
    }
}
