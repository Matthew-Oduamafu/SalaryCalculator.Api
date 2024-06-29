using System.Text.Json.Serialization;

namespace SalaryCalculator.Api.Models.Dtos;

public class SalaryResponseDto
{
    public decimal GrossSalary { get; set; }
    public decimal BasicSalary { get; set; }
    [JsonIgnore]
    public decimal ComputedNetSalary { get; set; }
    public decimal TotalPayeTax { get; set; }
    public decimal EmployeePensionContribution { get; set; }
    public decimal EmployerPensionContribution { get; set; }
}