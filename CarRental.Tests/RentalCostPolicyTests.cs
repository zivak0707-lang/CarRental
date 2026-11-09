using CarRental.Core.Pricing;

namespace CarRental.Tests;

public class RentalCostPolicyTests
{
    [Theory]
    [InlineData(0.01, 1, false, 0.01)]
    [InlineData(1000, 1, false, 1000)]
    [InlineData(1000, 1, true, 1200)]
    [InlineData(1000, 4, false, 1000)]
    [InlineData(1000, 5, false, 900)]
    [InlineData(1000, 5, true, 1100)]
    [InlineData(333.33, 5, true, 366.66)]
    public void DailyCostFor_Boundaries_MatchesTable(
        double dailyRate, int days, bool isWeekend, double expected)
    {
        // Act
        decimal actual = RentalCostPolicy.DailyCostFor((decimal)dailyRate, days, isWeekend);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-0.01)]
    [InlineData(-1000)]
    public void DailyCostFor_InvalidRate_ThrowsOutOfRange(double dailyRate)
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => RentalCostPolicy.DailyCostFor((decimal)dailyRate, 3, false));

        // Assert
        Assert.Equal("dailyRate", ex.ParamName);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void DailyCostFor_InvalidDays_ThrowsOutOfRange(int days)
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => RentalCostPolicy.DailyCostFor(1000m, days, false));

        // Assert
        Assert.Equal("days", ex.ParamName);
    }

    [Fact]
    public void DailyCostFor_WeekendOverSevenDays_TariffNotApplied()
    {
        // Arrange
        int days = 8;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(1000m, days, true);

        // Assert
        Assert.Equal(900m, actual);
    }
}
