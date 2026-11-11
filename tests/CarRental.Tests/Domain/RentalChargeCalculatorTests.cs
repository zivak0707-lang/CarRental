using CarRental.Domain.Pricing;

namespace CarRental.Tests.Domain;

/// <summary>Профільне правило: таблиця рішень docs/spec.md, розділ 4 — рядок на випадок.</summary>
public class RentalChargeCalculatorTests
{
    private static readonly DateTimeOffset _pickedUp =
        new(2026, 11, 2, 10, 0, 0, TimeSpan.FromHours(2));
    private readonly RentalChargeCalculator _calculator = new(new StandardLateFeePolicy());

    // Планове повернення — через 72 год (3 доби) після видачі; тариф 900 грн
    [Theory]
    [InlineData(120, 1, 900, 0)]
    [InlineData(1440, 1, 900, 0)]
    [InlineData(1441, 2, 1800, 0)]
    [InlineData(4320, 3, 2700, 0)]
    [InlineData(4379, 3, 2700, 0)]
    [InlineData(4380, 3, 2700, 0)]
    [InlineData(4381, 4, 3600, 450)]
    [InlineData(5760, 4, 3600, 450)]
    [InlineData(5761, 5, 4500, 900)]
    [InlineData(7200, 5, 4500, 900)]
    public void Calculate_DecisionTableRow_ReturnsDaysRentAndFee(
        int minutes, int days, int rent, int fee)
    {
        var stay = new StayPeriod(_pickedUp, _pickedUp.AddMinutes(minutes), _pickedUp.AddDays(3));

        var charge = _calculator.Calculate(900m, stay);

        Assert.Equal(days, charge.BillableDays);
        Assert.Equal((decimal)rent, charge.RentCost);
        Assert.Equal((decimal)fee, charge.LateFee);
        Assert.Equal((decimal)(rent + fee), charge.Total);
    }

    [Fact]
    public void Calculate_HalfKopeckInFee_RoundsAwayFromZero()
    {
        DateTimeOffset planned = _pickedUp.AddDays(3);
        var stay = new StayPeriod(_pickedUp, planned.AddHours(2), planned);

        var charge = _calculator.Calculate(999.99m, stay);

        Assert.Equal(3999.96m, charge.RentCost);
        Assert.Equal(500.00m, charge.LateFee);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Calculate_RateNotPositive_Throws(int rate)
    {
        var stay = new StayPeriod(_pickedUp, _pickedUp.AddDays(1), _pickedUp.AddDays(1));

        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Calculate(rate, stay));
    }

    [Fact]
    public void Calculate_ReturnNotAfterPickUp_Throws()
    {
        var stay = new StayPeriod(_pickedUp, _pickedUp, _pickedUp.AddDays(1));

        Assert.Throws<ArgumentOutOfRangeException>(() => _calculator.Calculate(900m, stay));
    }
}
