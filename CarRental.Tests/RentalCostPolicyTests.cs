using CarRental.Core.Pricing;

namespace CarRental.Tests;

public class RentalCostPolicyTests
{
    [Fact]
    public void DailyCostFor_NegativeRate_Throws()
    {
        // Arrange
        decimal dailyRate = -0.01m;

        // Act
        void Act() => RentalCostPolicy.DailyCostFor(dailyRate, 3, false);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void DailyCostFor_ZeroDays_Throws()
    {
        // Arrange
        int days = 0;

        // Act
        void Act() => RentalCostPolicy.DailyCostFor(1000m, days, false);

        // Assert
        Assert.Throws<ArgumentOutOfRangeException>(Act);
    }

    [Fact]
    public void DailyCostFor_MinimalRate_ReturnsSameRate()
    {
        // Arrange
        decimal dailyRate = 0.01m;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(dailyRate, 1, false);

        // Assert
        Assert.Equal(0.01m, actual);
    }

    [Fact]
    public void DailyCostFor_WeekdayOneDay_ReturnsBaseRate()
    {
        // Arrange
        decimal dailyRate = 1000m;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(dailyRate, 1, false);

        // Assert
        Assert.Equal(1000m, actual);
    }

    [Fact]
    public void DailyCostFor_WeekendOneDay_AddsSurcharge()
    {
        // Arrange
        decimal dailyRate = 1000m;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(dailyRate, 1, true);

        // Assert
        Assert.Equal(1200m, actual);
    }

    [Fact]
    public void DailyCostFor_FourDays_NoDiscount()
    {
        // Arrange
        int days = RentalCostPolicy.MultiDayFrom - 1;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(1000m, days, false);

        // Assert
        Assert.Equal(1000m, actual);
    }

    [Fact]
    public void DailyCostFor_FiveDays_AppliesDiscount()
    {
        // Arrange
        int days = RentalCostPolicy.MultiDayFrom;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(1000m, days, false);

        // Assert
        Assert.Equal(900m, actual);
    }

    [Fact]
    public void DailyCostFor_FiveDaysWeekend_CombinesBothRules()
    {
        // Arrange
        int days = RentalCostPolicy.MultiDayFrom;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(1000m, days, true);

        // Assert
        Assert.Equal(1100m, actual);
    }

    [Fact]
    public void DailyCostFor_NonRoundProduct_RoundsToKopecks()
    {
        // Arrange
        decimal dailyRate = 333.33m;

        // Act
        decimal actual = RentalCostPolicy.DailyCostFor(dailyRate, 5, true);

        // Assert
        Assert.Equal(366.66m, actual);
    }
}
