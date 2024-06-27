using Microsoft.Extensions.Options;
using SalaryCalculator.Api.Data.Entities;
using SalaryCalculator.Api.Models;
using SalaryCalculator.Api.Models.Dtos;
using SalaryCalculator.Api.Options;
using SalaryCalculator.Api.Repositories.Interfaces;
using SalaryCalculator.Api.Services.Interfaces;

namespace SalaryCalculator.Api.Services.Providers;

public class SalaryService : ISalaryService
{
    private readonly ILogger<SalaryService> _logger;
    private readonly ISalaryInfoRepository _salaryInfoRepo;
    private readonly PensionTierConfig _pensionTierConfig;

    public SalaryService(ILogger<SalaryService> logger, ISalaryInfoRepository salaryInfoRepo,
        IOptions<PensionTierConfig> pensionTierConfig)
    {
        _logger = logger;
        _salaryInfoRepo = salaryInfoRepo;
        _pensionTierConfig = pensionTierConfig.Value;
    }

    public async Task<GenericApiResponse<SalaryResponseDto>> CalculateGrossSalaryAsync(
        SalaryRequestDto request)
    {
        try
        {
            decimal grossSalary = 0;
            decimal basicSalary = 0;
            decimal totalPayeTax = 0;
            decimal employeePensionContribution = 0;
            decimal employerPensionContribution = 0;

            // Placeholder: Adjust basic salary and gross salary based on tax and pension contributions
            // Loop until the desired net salary is matched
            var totalAllowances = request.Allowances;
            var taxableIncome = request.DesiredNetSalary + totalAllowances;

            employeePensionContribution = CalculateEmployeePension(taxableIncome);
            var taxableIncomeAfterPension = taxableIncome - employeePensionContribution;

            var totalPAYETax = CalculatePayeTax(taxableIncomeAfterPension);

            grossSalary = taxableIncomeAfterPension + totalPAYETax;
            basicSalary = grossSalary - totalAllowances;
            employerPensionContribution = CalculateEmployerPension(basicSalary);
            

            var salaryInfo = new SalaryInfo
            {
                BasicSalary = basicSalary,
                GrossSalary = grossSalary,
                TotalPayeTax = totalPayeTax,
                EmployeePensionContribution = employeePensionContribution,
                EmployerPensionContribution = employerPensionContribution,
                DesiredNetSalary = request.DesiredNetSalary,
                Allowances = request.Allowances
            };

            await _salaryInfoRepo.AddSalaryInfoAsync(salaryInfo);

            var response = new SalaryResponseDto()
            {
                GrossSalary = grossSalary,
                BasicSalary = basicSalary,
                TotalPayeTax = totalPayeTax,
                EmployeePensionContribution = employeePensionContribution,
                EmployerPensionContribution = employerPensionContribution,
            };

            return response.ToOkApiResponse();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, ex.Message);
            return GenericApiResponse<SalaryResponseDto>.Default.ToInternalServerErrorApiResponse(
                "Oops! Something went wrong. Please try again later.");
        }
    }
    
    private decimal CalculateEmployeePension(decimal taxableIncome)
    {
        // Implement logic to calculate employee pension based on Tier 2 rate (5.5%)
        decimal tier1EmployeeContribution = taxableIncome * _pensionTierConfig.Tier1EmployeeRate * 0.01m;
        decimal tier2EmployeeContribution = taxableIncome * _pensionTierConfig.Tier2EmployeeRate * 0.01m;
        decimal tier3EmployeeContribution = taxableIncome * _pensionTierConfig.Tier3EmployeeRate * 0.01m;
        var employeePensionContribution =
            tier2EmployeeContribution + tier3EmployeeContribution + tier1EmployeeContribution;
        return employeePensionContribution;
    }

    private decimal CalculateEmployerPension(decimal basicSalary)
    {
        // Implement logic to calculate employer pension based on Tier 1 and Tier 3 rates (13% + 5%)
        var employerPensionContribution = basicSalary * _pensionTierConfig.Tier1EmployerRate  * 0.01m +
                                      basicSalary * _pensionTierConfig.Tier2EmployerRate  * 0.01m +
                                      basicSalary * _pensionTierConfig.Tier3EmployerRate  * 0.01m;
        // return basicSalary * (0.13 + 0.05);
        return employerPensionContribution;
    }

    private static decimal CalculatePayeTax(decimal taxableIncome)
    {
        decimal payeTax = 0;

        switch (taxableIncome)
        {
            case <= 490:
                return payeTax;
            case <= 600:
                payeTax += (taxableIncome - 490) * 0.05m;
                return payeTax;
            case <= 730:
                payeTax += 110 * 0.05m;
                payeTax += (taxableIncome - 600) * 0.10m;
                return payeTax;
            case <= 3896.67m:
                payeTax += 110 * 0.05m;
                payeTax += 130 * 0.10m;
                payeTax += (taxableIncome - 730) * 0.175m;
                return payeTax;
            case <= 19896.67m:
                payeTax += 110 * 0.05m;
                payeTax += 130 * 0.10m;
                payeTax += 3166.67m * 0.175m;
                payeTax += (taxableIncome - 3896.67m) * 0.25m;
                return payeTax;
            case <= 50416.67m:
                payeTax += 110 * 0.05m;
                payeTax += 130 * 0.10m;
                payeTax += 3166.67m * 0.175m;
                payeTax += 16000 * 0.25m;
                payeTax += (taxableIncome - 19896.67m) * 0.30m;
                return payeTax;
            default:
                payeTax += 110 * 0.05m;
                payeTax += 130 * 0.10m;
                payeTax += 3166.67m * 0.175m;
                payeTax += 16000 * 0.25m;
                payeTax += 30520 * 0.30m;
                payeTax += (taxableIncome - 50416.67m) * 0.35m;
                return payeTax;
        }
    }

}