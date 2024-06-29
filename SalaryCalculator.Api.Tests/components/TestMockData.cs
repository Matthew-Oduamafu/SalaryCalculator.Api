using SalaryCalculator.Api.Data.Entities;

namespace SalaryCalculator.Api.Tests.components;

public static class TestMockData
{
    public static List<SalaryInfo> MockSalaryInfos()
    {
        return new List<SalaryInfo>()
        {
            new SalaryInfo
            {
                Id = Guid.NewGuid().ToString("N"),
                Allowances = 0.0m,
                BasicSalary = 1000.0m,
                GrossSalary = 1000.0m,
                ComputedNetSalary = 1000.0m,
                DesiredNetSalary = 0.0m,
                TotalPayeTax = 0.0m,
                EmployeePensionContribution = 0,
                EmployerPensionContribution = 0,
                CreatedAt = DateTime.UtcNow,
            }
        };
    }
}