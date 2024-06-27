using System.ComponentModel.DataAnnotations;

namespace SalaryCalculator.Api.Options;

public class PensionTierConfig
{
    [Required] [Range(0, double.MaxValue)] public decimal Tier1EmployeeRate { get; set; }
    [Required] [Range(0, double.MaxValue)] public decimal Tier2EmployeeRate { get; set; }
    [Required] [Range(0, double.MaxValue)] public decimal Tier3EmployeeRate { get; set; }

    [Required] [Range(0, double.MaxValue)] public decimal Tier1EmployerRate { get; set; }
    [Required] [Range(0, double.MaxValue)] public decimal Tier2EmployerRate { get; set; }
    [Required] [Range(0, double.MaxValue)] public decimal Tier3EmployerRate { get; set; }
}