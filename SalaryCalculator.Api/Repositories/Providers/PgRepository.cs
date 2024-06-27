using SalaryCalculator.Api.Data;
using SalaryCalculator.Api.Repositories.Interfaces;

namespace SalaryCalculator.Api.Repositories.Providers;

public class PgRepository : IPgRepository
{
    private readonly ApplicationDbContext _dbContext;

    public PgRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }
}