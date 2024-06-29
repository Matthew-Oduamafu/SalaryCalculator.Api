



decimal desiredNet = 10463.38m;

decimal computedNet = 0;

decimal assumedGross = desiredNet;
decimal computedGross = 0;

decimal ssnit;
decimal tier2;
decimal taxableIncome;

decimal paye;

while (true)
{
    ssnit = 0.055m * assumedGross;
    tier2 = 0.05m * assumedGross;
    taxableIncome = assumedGross - tier2 - ssnit;
    paye = CalculatePayeTax(taxableIncome);
    
    
    computedNet = assumedGross - ssnit - tier2 - paye;
    if (Math.Round(computedNet, 1) == Math.Round(desiredNet, 1))
    {
        computedGross = assumedGross;
        break;
    }

    if (computedNet < desiredNet)
    {
        assumedGross += 0.1m;
    }
    else
    {
        assumedGross -= 0.1m;
    }
}

Console.WriteLine($"Desired Net: {desiredNet}");
Console.WriteLine($"Computed Net: {computedNet}");
Console.WriteLine($"assumed Gross: {assumedGross}");
Console.WriteLine($"Computed Gross: {computedGross}");
Console.WriteLine($"Employee ssnit: {ssnit}");
Console.WriteLine($"Employee tier2: {tier2}");
Console.WriteLine($"Paye: {paye}");


decimal CalculateTotalTax(decimal grossSalary)
{
    decimal totalTax = 0;
    
    decimal[] incomeBands = { 490, 110, 130, 3166.67m, 16000, 30520 };
    decimal[] taxRates = { 0, 0.05m, 0.10m, 0.175m, 0.25m, 0.30m, 0.35m };

    decimal remainingSalary = grossSalary;

    for (var i = 0; i < incomeBands.Length; i++)
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

static decimal CalculatePayeTax(decimal taxableIncome)
{
    decimal payeTax = 0;

    switch (taxableIncome)
    {
        case <= 490:
            return payeTax;
        case <= 600:
            payeTax += (taxableIncome - 490) * 0.05m;
            return payeTax;
        case <= 730:
            payeTax += 110 * 0.05m;
            payeTax += (taxableIncome - 600) * 0.10m;
            return payeTax;
        case <= 3896.67m:
            payeTax += 110 * 0.05m;
            payeTax += 130 * 0.10m;
            payeTax += (taxableIncome - 730) * 0.175m;
            return payeTax;
        case <= 19896.67m:
            payeTax += 110 * 0.05m;
            payeTax += 130 * 0.10m;
            payeTax += 3166.67m * 0.175m;
            payeTax += (taxableIncome - 3896.67m) * 0.25m;
            return payeTax;
        case <= 50416.67m:
            payeTax += 110 * 0.05m;
            payeTax += 130 * 0.10m;
            payeTax += 3166.67m * 0.175m;
            payeTax += 16000 * 0.25m;
            payeTax += (taxableIncome - 19896.67m) * 0.30m;
            return payeTax;
        default:
            payeTax += 110 * 0.05m;
            payeTax += 130 * 0.10m;
            payeTax += 3166.67m * 0.175m;
            payeTax += 16000 * 0.25m;
            payeTax += 30520 * 0.30m;
            payeTax += (taxableIncome - 50416.67m) * 0.35m;
            return payeTax;
    }
}