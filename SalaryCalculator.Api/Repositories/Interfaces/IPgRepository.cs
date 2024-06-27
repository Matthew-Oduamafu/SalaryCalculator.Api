namespace SalaryCalculator.Api.Repositories.Interfaces;

public interface IPgRepository
{
    Task<int> SaveChangesAsync();
}