using CarRental.Core.Rentals;
using CarRental.Core.Reporting;

namespace CarRental.Tests;

public class RentalReportTests
{
    [Fact]
    public void TextOf_RegularCustomer_ContainsTotalLine()
    {
        TotalRequest request = SampleRequests.RegularCustomer();
        TotalResult result = RentalTotalCalculator.Calculate(request);

        string text = RentalReport.TextOf(request, result);

        Assert.Contains("Разом: 1292.70", text);
    }

    [Fact]
    public void Print_ReportText_WritesToOutput()
    {
        StringWriter writer = new();
        RentalReportPrinter printer = new(writer);

        printer.Print("Разом: 10.00");

        Assert.Contains("10.00", writer.ToString());
    }
}
