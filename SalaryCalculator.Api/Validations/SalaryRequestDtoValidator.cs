using FluentValidation;
using SalaryCalculator.Api.Models.Dtos;

namespace SalaryCalculator.Api.Validations;

public class SalaryRequestDtoValidator : AbstractValidator<SalaryRequestDto>
{
    public SalaryRequestDtoValidator()
    {
        ClassLevelCascadeMode = CascadeMode.Stop;
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.DesiredNetSalary)
            .NotNull()
            .GreaterThan(0.0m)
            .LessThanOrEqualTo(decimal.MaxValue);

        RuleFor(x => x.Allowances)
            .NotNull()
            .GreaterThan(0.0m);
    }
}