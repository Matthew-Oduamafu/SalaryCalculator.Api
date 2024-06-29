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
    private readonly GRAConfig _graConfig;
    private readonly List<TaxBand> _taxBands = new();

    public SalaryService(ILogger<SalaryService> logger, ISalaryInfoRepository salaryInfoRepo,
        IOptions<PensionTierConfig> pensionTierConfig, IOptions<GRAConfig> graConfig)
    {
        _logger = logger;
        _salaryInfoRepo = salaryInfoRepo;
        _pensionTierConfig = pensionTierConfig.Value;
        _graConfig = graConfig.Value;

        for (var i = 0; i < _graConfig.IncomeBands.Length; i++)
        {
            _taxBands.Add(new TaxBand
            {
                UpperLimit = _graConfig.IncomeBands[i],
                Rate = _graConfig.TaxRates[i]
            });
        }
    }

    public async Task<GenericApiResponse<SalaryResponseDto>> CalculateGrossSalaryAsync(
        SalaryRequestDto request)
    {
        try
        {
            decimal grossSalary = request.DesiredNetSalary;
            decimal netSalary = 0;
            decimal totalAllowances = request.Allowance;
            decimal totalEmployeePensionContribution;

            // Reverse calculate the gross salary to match desired net salary
            while (true)
            {
                // Calculate employee pension contributions
                totalEmployeePensionContribution = TotalEmployeePensionContribution(grossSalary);

                // Calculate taxable income
                decimal taxableIncome = grossSalary - totalEmployeePensionContribution;

                // Calculate PAYE tax
                decimal payeTax = CalculatePayeTax(taxableIncome);

                // Calculate net salary
                netSalary = grossSalary - payeTax - totalEmployeePensionContribution;

                if (Math.Abs(netSalary - request.DesiredNetSalary) < 0.01m)
                    break;

                grossSalary += 0.01m;
            }

            // Calculate employer pension contributions
            var totalEmployerPensionContribution = TotalEmployerPensionContribution(grossSalary);

            var baseSalary = grossSalary - totalAllowances;
            var totalPayeTax = CalculatePayeTax(grossSalary - totalEmployeePensionContribution);


            var salaryInfo = new SalaryInfo
            {
                BasicSalary = baseSalary,
                GrossSalary = grossSalary,
                ComputedNetSalary = netSalary,
                TotalPayeTax = totalPayeTax,
                EmployeePensionContribution = totalEmployeePensionContribution,
                EmployerPensionContribution = totalEmployerPensionContribution,
                DesiredNetSalary = request.DesiredNetSalary,
                Allowances = request.Allowance
            };

            await _salaryInfoRepo.AddSalaryInfoAsync(salaryInfo);

            var response = new SalaryResponseDto
            {
                GrossSalary = Math.Round(grossSalary, 2),
                BasicSalary = Math.Round(baseSalary, 2),
                ComputedNetSalary = Math.Round(netSalary, 2),
                TotalPayeTax = Math.Round(totalPayeTax, 2),
                EmployeePensionContribution = Math.Round(totalEmployeePensionContribution, 2),
                EmployerPensionContribution = Math.Round(totalEmployerPensionContribution, 2)
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

    private decimal TotalEmployeePensionContribution(decimal grossSalary)
    {
        decimal employeePensionContributionTier1 = grossSalary * _pensionTierConfig.Tier1EmployeeRate / 100;
        decimal employeePensionContributionTier2 = grossSalary * _pensionTierConfig.Tier2EmployeeRate / 100;
        decimal employeePensionContributionTier3 = grossSalary * _pensionTierConfig.Tier3EmployeeRate / 100;
        var totalEmployeePensionContribution = employeePensionContributionTier1 + employeePensionContributionTier2 + employeePensionContributionTier3;
        return totalEmployeePensionContribution;
    }

    private decimal TotalEmployerPensionContribution(decimal grossSalary)
    {
        decimal employerPensionContributionTier1 = grossSalary * _pensionTierConfig.Tier1EmployerRate / 100;
        decimal employerPensionContributionTier2 = grossSalary * _pensionTierConfig.Tier2EmployerRate / 100;
        decimal employerPensionContributionTier3 = grossSalary * _pensionTierConfig.Tier3EmployerRate / 100;
        decimal totalEmployerPensionContribution =
            employerPensionContributionTier1 + employerPensionContributionTier2 + employerPensionContributionTier3;
        return totalEmployerPensionContribution;
    }

    private decimal CalculatePayeTax(decimal taxableIncome)
    {
        decimal totalTax = 0;
        decimal remainingIncome = taxableIncome;

        foreach (var band in _taxBands)
        {
            if (remainingIncome <= 0)
                break;

            decimal bandIncome = Math.Min(remainingIncome, band.UpperLimit);
            totalTax += bandIncome * band.Rate;
            remainingIncome -= bandIncome;
        }

        return totalTax;
    }
}