using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<TaskContext>
{
    public TaskContext CreateDbContext(string[] args)
    {
        IConfigurationRoot configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var builder = new DbContextOptionsBuilder<TaskContext>();
        var connectionString = configuration.GetConnectionString("TaskQueueGrupo3DBConnection");

        builder.UseSqlServer(connectionString);

        return new TaskContext(builder.Options);
    }
}
