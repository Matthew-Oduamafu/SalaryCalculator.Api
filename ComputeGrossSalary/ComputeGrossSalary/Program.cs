using System;


double desiredNetSalary = 10463.38;
double allowance = 600;
double totalNetSalary = desiredNetSalary + allowance;
double employeePensionRate = 0.105; // 10.5% for Tier 2 and Tier 3
double employerPensionRate = 0.18; // 13% for Tier 1 and 5% for Tier 3

double grossSalary = CalculateGrossSalary(totalNetSalary, employeePensionRate);

double employeePensionContribution = grossSalary * employeePensionRate;
double employerPensionContribution = grossSalary * employerPensionRate;

double basicSalary = grossSalary - allowance;
double totalPayeTax = CalculateTotalTax(grossSalary);

Console.WriteLine($"Desired Net Salary: GH¢ {desiredNetSalary}");
Console.WriteLine($"Allowance: GH¢ {allowance}");
Console.WriteLine($"Gross Salary: GH¢ {grossSalary:F2}");
Console.WriteLine($"Basic Salary: GH¢ {basicSalary:F2}");
Console.WriteLine($"Total PAYE Tax: GH¢ {totalPayeTax:F2}");
Console.WriteLine($"Employee Pension Contribution: GH¢ {employeePensionContribution:F2}");
Console.WriteLine($"Employer Pension Contribution: GH¢ {employerPensionContribution:F2}");


static double CalculateGrossSalary(double netSalary, double employeePensionRate)
{
    // This function will calculate the gross salary based on the net salary
    double grossSalary = netSalary / (1 - employeePensionRate);
    double tax = CalculateTotalTax(grossSalary);
    double employeePensionContribution = grossSalary * employeePensionRate;

    while (grossSalary - tax - employeePensionContribution < netSalary)
    {
        grossSalary += 0.01;
        tax = CalculateTotalTax(grossSalary);
        employeePensionContribution = grossSalary * employeePensionRate;
    }

    return grossSalary;
}

static double CalculateTotalTax(double grossSalary)
{
    double totalTax = 0;
    double[] incomeBands = { 490, 110, 130, 3166.67, 16000, 30520 };
    double[] taxRates = { 0, 0.05, 0.10, 0.175, 0.25, 0.30, 0.35 };

    double remainingSalary = grossSalary;

    for (int i = 0; i < incomeBands.Length; i++)
    {
        if (remainingSalary > incomeBands[i])
        {
            totalTax += incomeBands[i] * taxRates[i];
            remainingSalary -= incomeBands[i];
        }
        else
        {
            totalTax += remainingSalary * taxRates[i];
            remainingSalary = 0;
            break;
        }
    }

    if (remainingSalary > 0)
    {
        totalTax += remainingSalary * taxRates[taxRates.Length - 1];
    }

    return totalTax;
}