using CarRental.Application.Rentals;
using CarRental.Domain.Errors;
using CarRental.Domain.Pricing;
using CarRental.Domain.Rentals;
using CarRental.Tests.Doubles;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests.Application;

/// <summary>Сценарії-ядра варіанта: UC-02 «Видати автомобіль» і UC-03 «Закрити оренду».</summary>
public class HandoverServiceTests
{
    private readonly FakeRentalRepository _rentals = new();
    private readonly FixedClock _clock = new(Sample.Now);
    private readonly HandoverService _service;

    public HandoverServiceTests()
    {
        var calculator = new RentalChargeCalculator(new StandardLateFeePolicy());
        _service = new HandoverService(
            _rentals, calculator, _clock, NullLogger<HandoverService>.Instance);
    }

    [Fact]
    public void PickUp_Reserved_BecomesActiveAtCurrentTime()
    {
        _rentals.Add(Sample.Reserved(1001, "AA1001AA"));

        var result = _service.PickUp(1001);

        Assert.Equal(RentalStatus.Active, result.Value!.Status);
        Assert.Equal(Sample.Now, result.Value.PickedUpAt);
        Assert.Equal(1, _rentals.SaveCount);
    }

    [Fact]
    public void PickUp_UnknownNumber_Fails()
    {
        Assert.Equal("Договору № 5 не знайдено.", _service.PickUp(5).Error);
    }

    [Fact]
    public void PickUp_PlannedPeriodPassed_Fails()
    {
        _rentals.Add(Sample.Reserved(1001, "AA1001AA", startInDays: -3, days: 2));

        var result = _service.PickUp(1001);

        Assert.Equal("Період оренди минув; потрібне нове бронювання.", result.Error);
    }

    [Fact]
    public void PickUp_AlreadyActive_ThrowsDomainException()
    {
        _rentals.Add(Sample.Active(1001, "AA1001AA", Sample.Now.AddHours(-2)));

        Assert.Throws<InvalidRentalStateException>(() => _service.PickUp(1001));
        Assert.Equal(0, _rentals.SaveCount);
    }

    [Fact]
    public void Close_LateBeyondGrace_ChargesDaysAndLateFee()
    {
        _rentals.Add(Sample.Active(1001, "AA1001AA", Sample.Now, days: 3));
        _clock.Advance(TimeSpan.FromDays(3) + TimeSpan.FromHours(5));

        var result = _service.Close(1001);

        Assert.Equal(4, result.Value!.BillableDays);
        Assert.Equal(450m, result.Value.LateFee);
        Assert.Equal(4050m, result.Value.Total);
        Assert.Equal(RentalStatus.Closed, _rentals.Find(1001)!.Status);
    }

    [Fact]
    public void Close_UnknownNumber_Fails()
    {
        Assert.Equal("Договору № 9 не знайдено.", _service.Close(9).Error);
    }

    [Fact]
    public void Close_Reserved_ThrowsDomainException()
    {
        _rentals.Add(Sample.Reserved(1001, "AA1001AA"));

        Assert.Throws<InvalidRentalStateException>(() => _service.Close(1001));
    }
}
