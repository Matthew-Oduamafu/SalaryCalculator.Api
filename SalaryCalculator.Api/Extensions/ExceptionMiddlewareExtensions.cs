using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
using SalaryCalculator.Api.CustomMiddleware;
using SalaryCalculator.Api.Data;

namespace SalaryCalculator.Api.Extensions;

public static class ExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder app, ILogger logger)
    {
        return app.UseMiddleware<ExceptionMiddleware>(logger);
    }
}