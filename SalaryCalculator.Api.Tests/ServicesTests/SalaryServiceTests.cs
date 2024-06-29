using FluentAssertions;
using Microsoft.AspNetCore.Http;
using SalaryCalculator.Api.Options;
using SalaryCalculator.Api.Services.Interfaces;
using SalaryCalculator.Api.Tests.components;
using Microsoft.Extensions.Options;
using SalaryCalculator.Api.Models.Dtos;
using SalaryCalculator.Api.Services.Providers;

namespace SalaryCalculator.Api.Tests.ServicesTests;

public class SalaryServiceTests : IClassFixture<DiFixture>
{
    private readonly ISalaryService _sut;

    private readonly IOptions<PensionTierConfig> _pensionOptions = Microsoft.Extensions.Options.Options.Create(
        new PensionTierConfig()
        {
            Tier1EmployeeRate = 0.0m,
            Tier1EmployerRate = 13m,

            Tier2EmployeeRate = 5.5m,
            Tier2EmployerRate = 0,

            Tier3EmployeeRate = 5.0m,
            Tier3EmployerRate = 5.0m
        });

    public SalaryServiceTests(DiFixture fixture)
    {
        decimal[] incomeBands = [490, 110, 130, 3166.67m, 16000, 30520];
        decimal[] taxRates = [0, 0.05m, 0.10m, 0.175m, 0.25m, 0.30m, 0.35m];

        var graOptions = Microsoft.Extensions.Options.Options.Create(
            new GRAConfig
            {
                IncomeBands = incomeBands,
                TaxRates = taxRates
            });

        _sut = new SalaryService(fixture.Logger, fixture.SalaryInfoRepo, _pensionOptions, graOptions);
    }

    [Fact]
    public async Task CalculateGrossSalaryAsync_Should_Return_Success_With_Code_200_When_Request_Is_Valid()
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = 600,
            DesiredNetSalary = 10463.38m
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Code.Should().Be(StatusCodes.Status200OK);
    }

    
    [Theory]
    [InlineData(7500, 0)]
    [InlineData(6318, 595)]
    [InlineData(10463.38, 600)]
    public async Task CalculateGrossSalaryAsync_Should_Compute_NetSalary_Equals_Desired_NetSalary(decimal desiredNet, decimal allowanace)
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = allowanace,
            DesiredNetSalary = desiredNet
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data!.ComputedNetSalary.Should().BeInRange(desiredNet - 0.1m, desiredNet + 0.1m);  // computed net salary should be equal to the desired net salary
    }

    
    [Theory]
    [InlineData(7500, 0, 10575.04)]
    [InlineData(7028, 400, 9871.87)]
    public async Task CalculateGrossSalaryAsync_Should_Compute_GrossSalary_With_NetSalary(decimal desiredNet, decimal allowanace, decimal gross)
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = allowanace,
            DesiredNetSalary = desiredNet
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data!.GrossSalary.Should().Be(gross);
    }

    
    [Theory]
    [InlineData(7500, 0, 10575.04)]
    [InlineData(7028, 400, 9471.87)]
    public async Task CalculateGrossSalaryAsync_Should_Compute_BaseSalary_With_Valid_NetSalary(decimal desiredNet, decimal allowanace, decimal baseSalary)
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = allowanace,
            DesiredNetSalary = desiredNet
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data!.BasicSalary.Should().Be(baseSalary);
    }

    
    [Theory]
    [InlineData(7500, 0, 1964.66)]
    [InlineData(7028, 400, 1807.33)]
    public async Task CalculateGrossSalaryAsync_Should_Compute_PayeTax_With_Valid_NetSalary(decimal desiredNet, decimal allowanace, decimal payeTax)
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = allowanace,
            DesiredNetSalary = desiredNet
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data?.TotalPayeTax.Should().Be(payeTax);
    }
    
    [Theory]
    [InlineData(7500, 0, 1110.38)]
    [InlineData(7028, 400, 1036.55)]
    public async Task CalculateGrossSalaryAsync_Should_Compute_EmployeeContribution_With_Valid_NetSalary(decimal desiredNet, decimal allowanace, decimal empCon)
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = allowanace,
            DesiredNetSalary = desiredNet
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data!.EmployeePensionContribution.Should().Be(empCon);
    }
    
    [Theory]
    [InlineData(7500, 0, 1903.51)]
    [InlineData(7028, 400, 1776.94)]
    public async Task CalculateGrossSalaryAsync_Should_Compute_EmployerContribution_With_Valid_NetSalary(decimal desiredNet, decimal allowanace, decimal empYerCon)
    {
        // Arrange
        var request = new SalaryRequestDto
        {
            Allowance = allowanace,
            DesiredNetSalary = desiredNet
        };

        // Act
        var result = await _sut.CalculateGrossSalaryAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data!.EmployerPensionContribution.Should().Be(empYerCon);
    }
}