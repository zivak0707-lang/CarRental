using CarRental.Domain.Pricing;

namespace CarRental.Tests.Seminar;

/// <summary>
/// Семінар 2, тема 20: однакове покриття, різна користь.
/// Обидва тести виконують ті самі рядки й гілки розрахунку.
/// </summary>
public class CoverageDemoTests
{
    private static readonly DateTimeOffset _pickedUp =
        new(2026, 11, 2, 10, 0, 0, TimeSpan.FromHours(2));

    private readonly RentalChargeCalculator _calc =
        new(new StandardLateFeePolicy());

    // БУЛО: 100 % рядків і гілок калькулятора, жодної перевірки
    [Fact]
    public void Calculate_ThreeStays_RunsWithoutAsserts()
    {
        _calc.Calculate(900m, Stay(minutesAfterPlan: 0));
        _calc.Calculate(900m, Stay(minutesAfterPlan: 30));
        _calc.Calculate(900m, Stay(minutesAfterPlan: 61));
    }

    // СТАЛО: ті самі виклики, але кожен результат перевірено
    [Theory]
    [InlineData(0, 2700)]
    [InlineData(30, 2700)]
    [InlineData(61, 4050)]
    public void Calculate_ThreeStays_ReturnsTotal(
        int minutesAfterPlan, int total)
    {
        var charge = _calc.Calculate(900m, Stay(minutesAfterPlan));

        Assert.Equal((decimal)total, charge.Total);
    }

    private static StayPeriod Stay(int minutesAfterPlan)
    {
        DateTimeOffset planned = _pickedUp.AddDays(3);
        return new StayPeriod(
            _pickedUp, planned.AddMinutes(minutesAfterPlan), planned);
    }
}
