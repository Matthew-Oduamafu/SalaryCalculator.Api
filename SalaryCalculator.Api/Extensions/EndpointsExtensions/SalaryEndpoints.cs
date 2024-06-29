using Microsoft.AspNetCore.Mvc;
using SalaryCalculator.Api.CustomFilters;
using SalaryCalculator.Api.Models;
using SalaryCalculator.Api.Models.Dtos;
using SalaryCalculator.Api.Services.Interfaces;

namespace SalaryCalculator.Api.Extensions.EndpointsExtensions;

public static class SalaryEndpoints
{
    public static void MapSalaryEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.MapGroup("/api/salary")
            .WithTags("Salary");

        group.MapPost("gross",
                async ([FromServices] ISalaryService salaryService, [FromBody] SalaryRequestDto request) =>
                {
                    var response = await salaryService.CalculateGrossSalaryAsync(request);
                    return response.ToActionResult();
                })
            .AddEndpointFilter<ValidationFilter<SalaryRequestDto>>()
            .WithName("CalculateGrossSalary")
            .Produces<IGenericApiResponse<SalaryResponseDto>>()
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status400BadRequest)
            .Produces<IGenericApiResponse<object>>(StatusCodes.Status500InternalServerError)
            .WithSummary("Calculate gross salary")
            .WithDescription("Please provide a valid request.<br/>Kindly payload `schema` for payload requirements.")
            .WithOpenApi();
    }
}