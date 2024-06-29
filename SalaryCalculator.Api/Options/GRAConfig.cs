using System.ComponentModel.DataAnnotations;

namespace SalaryCalculator.Api.Options;

public class GRAConfig
{
    [Required] [MinLength(1)] public decimal[] IncomeBands { get; set; }
    [Required] [MinLength(1)] public decimal[] TaxRates { get; set; }
}