using SalaryCalculator.Api.Data;
using SalaryCalculator.Api.Data.Entities;
using SalaryCalculator.Api.Repositories.Interfaces;

namespace SalaryCalculator.Api.Repositories.Providers;

public class SalaryInfoRepository : ISalaryInfoRepository
{
    private readonly ApplicationDbContext _context;

    public SalaryInfoRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<bool> AddSalaryInfoAsync(SalaryInfo salaryInfo)
    {
        await _context.SalaryInfos.AddAsync(salaryInfo);
        var saved = await _context.SaveChangesAsync() > 0;
        return saved;
    }

    public async Task<SalaryInfo?> GetSalaryInfoByIdAsync(string id)
    {
        var salaryInfo = await _context.SalaryInfos.FindAsync(id);
        return salaryInfo;
    }

    public async Task<bool> UpdateSalaryInfoAsync(SalaryInfo salaryInfo)
    {
        _context.SalaryInfos.Update(salaryInfo);
        var updated = await _context.SaveChangesAsync() > 0;
        return updated;
    }

    public IQueryable<SalaryInfo> GetSalaryInfosAsQuerable()
    {
        return _context.SalaryInfos.AsQueryable();
    }
}