using SalaryCalculator.Api.Data.Entities;

namespace SalaryCalculator.Api.Repositories.Interfaces;

public interface ISalaryInfoRepository
{
    Task<bool> AddSalaryInfoAsync(SalaryInfo salaryInfo);
    Task<SalaryInfo?> GetSalaryInfoByIdAsync(string id);
    Task<bool> UpdateSalaryInfoAsync(SalaryInfo salaryInfo);
    IQueryable<SalaryInfo> GetSalaryInfosAsQuerable();
}