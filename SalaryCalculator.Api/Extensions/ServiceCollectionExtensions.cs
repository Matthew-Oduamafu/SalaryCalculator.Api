using System.Reflection;
using Asp.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using SalaryCalculator.Api.Data;
using SalaryCalculator.Api.Options;
using SalaryCalculator.Api.Repositories.Interfaces;
using SalaryCalculator.Api.Repositories.Providers;
using SalaryCalculator.Api.Services.Interfaces;
using SalaryCalculator.Api.Services.Providers;

namespace SalaryCalculator.Api.Extensions;

#pragma warning disable CS1591
public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<ISalaryInfoRepository, SalaryInfoRepository>();
        services.AddScoped<IPgRepository, PgRepository>();
        return services;
    }

    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<ISalaryService, SalaryService>();
        return services;
    }

    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddOptions<DatabaseConfig>()
            .BindConfiguration(nameof(DatabaseConfig))
            .Configure(c => { c.DbConnectionString = configuration.GetConnectionString("DbConnection")!; })
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddDbContext<ApplicationDbContext>((serviceProvider, options) =>
        {
            var dbConfig = serviceProvider.GetRequiredService<IOptions<DatabaseConfig>>().Value;
            options.UseMySql(dbConfig.DbConnectionString, ServerVersion.AutoDetect(dbConfig.DbConnectionString), builder =>
            {
                if (dbConfig.EnableRetryOnFailure)
                {
                    builder.EnableRetryOnFailure(dbConfig.MaxRetryCount,
                        TimeSpan.FromMilliseconds(dbConfig.MaxRetryDelay),
                        dbConfig.ErrorNumbersToAdd);
                }

                builder.CommandTimeout(dbConfig.CommandTimeout);
            });
        });
        
        return services;
    }

    public static IServiceCollection AddControllerConfiguration(this IServiceCollection services)
    {
        services.Configure<RouteOptions>(options =>
        {
            options.LowercaseUrls = true;
            options.LowercaseQueryStrings = true;
        });
        
        return services;
    }
    public static IServiceCollection AddConfigurations(this IServiceCollection services)
    {
        services.AddOptions<PensionTierConfig>()
            .BindConfiguration(nameof(PensionTierConfig))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddOptions<GRAConfig>()
            .BindConfiguration(nameof(GRAConfig))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        return services;
    }
    
    public static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.CustomSchemaIds(type => type.FullName ?? type.Name);
            
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Gross Salary Calculator",
                Version = "v1",
                Description = "This is a simple application for computing gross salary given the desired net salary",
                Contact = new OpenApiContact
                {
                    Email = "mattoduamafu@gmail.com",
                    Extensions = { },
                    Name = "Matthew",
                    Url = new Uri($"https://www.linkedin.com/in/matthew-oduamafu-42a1551a7/")
                },
                License = new OpenApiLicense
                {
                    Name = "License",
                    Url = new Uri($"https://github.com/")
                },
                Extensions = { },
                TermsOfService = new Uri($"https://github.com/Matthew-Oduamafu")
            });

            c.EnableAnnotations();
            
            // Optionally, add XML comments:
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
    }
}