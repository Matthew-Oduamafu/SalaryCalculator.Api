namespace SalaryCalculator.Api.Models.Dtos;

public class SalaryRequestDto
{
    public decimal DesiredNetSalary { get; set; }
    public decimal Allowance { get; set; }
}