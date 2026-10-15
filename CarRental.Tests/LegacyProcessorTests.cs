using CarRental.Core.Domain;
using CarRental.Core.Legacy;

namespace CarRental.Tests;

public class LegacyProcessorTests
{
    [Fact]
    public void Process_RegularCustomer_Returns1292()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 12, DailyRate = 100m },
            new RentalLine { Plate = "KA5678BB", Days = 1, DailyRate = 250m },
        };

        decimal total = LegacyProcessor.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string next);

        Assert.Equal(1292.70m, total);
        Assert.Equal("Confirmed", next);
    }

    [Fact]
    public void Process_SmallRental_AddsDelivery()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 2, DailyRate = 200m },
        };

        decimal total = LegacyProcessor.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            "Confirmed",
            false,
            60m,
            out string next);

        Assert.Equal(420m, total);
        Assert.Equal("Active", next);
    }

    [Fact]
    public void Process_NoLines_ReturnsMinusOne()
    {
        decimal total = LegacyProcessor.Process(
            3,
            "Коваль",
            "k@ex.com",
            false,
            new List<RentalLine>(),
            new DateOnly(2026, 3, 10),
            string.Empty,
            "New",
            false,
            60m,
            out string next);

        Assert.Equal(-1m, total);
        Assert.Equal("New", next);
    }
}
