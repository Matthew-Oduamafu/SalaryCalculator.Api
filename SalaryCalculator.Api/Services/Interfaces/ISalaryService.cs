using SalaryCalculator.Api.Models;
using SalaryCalculator.Api.Models.Dtos;

namespace SalaryCalculator.Api.Services.Interfaces;

public interface ISalaryService
{
    public Task<GenericApiResponse<SalaryResponseDto>> CalculateGrossSalaryAsync(SalaryRequestDto request);
}