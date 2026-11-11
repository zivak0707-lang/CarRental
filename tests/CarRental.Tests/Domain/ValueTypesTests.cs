using CarRental.Domain.Pricing;
using CarRental.Domain.Rentals;

namespace CarRental.Tests.Domain;

public class ValueTypesTests
{
    private static readonly DateTimeOffset _start =
        new(2026, 11, 10, 10, 0, 0, TimeSpan.FromHours(2));

    // Періоди напіввідкриті: спільна межа не є перетином
    [Theory]
    [InlineData(0, 3, true)]
    [InlineData(2, 5, true)]
    [InlineData(3, 5, false)]
    [InlineData(-2, 0, false)]
    public void Overlaps_OtherPeriod_DetectsIntersection(int fromDay, int toDay, bool expected)
    {
        var period = new RentalPeriod(_start, _start.AddDays(3));
        var other = new RentalPeriod(_start.AddDays(fromDay), _start.AddDays(toDay));

        Assert.Equal(expected, period.Overlaps(other));
    }

    [Theory]
    [InlineData(9, false)]
    [InlineData(10, true)]
    [InlineData(12, true)]
    [InlineData(13, false)]
    public void Covers_Day_TrueBetweenStartDateAndEndDate(int dayOfMonth, bool expected)
    {
        var period = new RentalPeriod(_start, _start.AddDays(3));

        Assert.Equal(expected, period.Covers(new DateOnly(2026, 11, dayOfMonth)));
    }

    [Fact]
    public void RentalPeriod_EndNotAfterStart_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RentalPeriod(_start, _start));
    }

    [Theory]
    [InlineData("10.005", "10.01")]
    [InlineData("10.004", "10.00")]
    [InlineData("-10.005", "-10.01")]
    public void RoundToKopecks_Amount_RoundsHalfAwayFromZero(string amount, string expected)
    {
        decimal value = decimal.Parse(amount, System.Globalization.CultureInfo.InvariantCulture);

        Assert.Equal(
            decimal.Parse(expected, System.Globalization.CultureInfo.InvariantCulture),
            Money.RoundToKopecks(value));
    }
}
