namespace SalaryCalculator.Api.Data.Entities;

public class SalaryInfo : BaseEntity
{
    public decimal DesiredNetSalary { get; set; }
    public decimal Allowances { get; set; }
    public decimal GrossSalary { get; set; }
    public decimal BasicSalary { get; set; }
    public decimal ComputedNetSalary { get; set; }
    public decimal TotalPayeTax { get; set; }
    public decimal EmployeePensionContribution { get; set; }
    public decimal EmployerPensionContribution { get; set; }
}