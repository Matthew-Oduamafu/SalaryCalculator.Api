using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using SalaryCalculator.Api.Data.Entities;

namespace SalaryCalculator.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<SalaryInfo> SalaryInfos => Set<SalaryInfo>();
    
}

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        // get environment\
        var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
        
        // build config
        var config = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../SalaryCalculator.Api"))
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment}.json", optional: true)
            .AddEnvironmentVariables()
            .Build();
        
        // get connection string
        var connectionString = config.GetConnectionString("DbConnection");
        // dbContext builder 
        var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
        // use mysql
        builder.UseMySql(connectionString!, ServerVersion.AutoDetect(connectionString));
        // return dbContext
        return new ApplicationDbContext(builder.Options);
    }
}