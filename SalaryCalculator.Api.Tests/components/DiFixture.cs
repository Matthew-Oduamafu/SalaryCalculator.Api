using Microsoft.Extensions.Logging;
using NSubstitute;
using SalaryCalculator.Api.Data.Entities;
using SalaryCalculator.Api.Repositories.Interfaces;
using SalaryCalculator.Api.Services.Providers;

namespace SalaryCalculator.Api.Tests.components;

public class DiFixture
{
    public ISalaryInfoRepository SalaryInfoRepo { get; private set; }
    public ILogger<SalaryService> Logger { get; private set; }

    public DiFixture()
    {
        SalaryInfoRepo = Substitute.For<ISalaryInfoRepository>();
        Logger = Substitute.For<ILogger<SalaryService>>();


        // mocking salary info methods

        SalaryInfoRepo.AddSalaryInfoAsync(Arg.Any<SalaryInfo>())
            .ReturnsForAnyArgs(info =>
            {
                var payload = info.ArgAt<SalaryInfo>(0);
                return payload != null;
            });

        SalaryInfoRepo.GetSalaryInfoByIdAsync(Arg.Any<string>())
            .ReturnsForAnyArgs(info =>
            {
                var id = info.ArgAt<string>(0);
                return TestMockData.MockSalaryInfos().Find(s => s.Id.Equals(id));
            });

        SalaryInfoRepo.UpdateSalaryInfoAsync(Arg.Any<SalaryInfo>())
            .ReturnsForAnyArgs(info =>
            {
                var payload = info.ArgAt<SalaryInfo>(0);
                return payload != null;
            });

        SalaryInfoRepo.GetSalaryInfosAsQuerable()
            .ReturnsForAnyArgs(info => TestMockData.MockSalaryInfos().AsQueryable());
    }
}