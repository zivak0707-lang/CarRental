using CarRental.Application.Rentals;
using CarRental.Domain.Errors;
using CarRental.Domain.Rentals;
using CarRental.Tests.Doubles;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests.Application;

/// <summary>UC-01 і UC-04, зокрема барикада валідації на межі прикладного шару.</summary>
public class ReservationServiceTests
{
    private readonly FakeRentalRepository _rentals = new();
    private readonly ReservationService _service;

    public ReservationServiceTests()
    {
        _service = new ReservationService(
            _rentals,
            new FakeCatalog(),
            new FixedClock(Sample.Now),
            NullLogger<ReservationService>.Instance);
    }

    [Fact]
    public void Reserve_ValidRequest_CreatesReservedRentalWithClassRate()
    {
        var request = new ReserveRequest(1, " aa2001bb ", Sample.Now.AddDays(1), 3);

        var result = _service.Reserve(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(RentalStatus.Reserved, result.Value!.Status);
        Assert.Equal("AA2001BB", result.Value.Terms.CarPlate);
        Assert.Equal(1800m, result.Value.Terms.DailyRate);
        Assert.Equal(1, _rentals.SaveCount);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Reserve_EmptyPlate_FailsWithMessage(string? plate)
    {
        var result = _service.Reserve(new ReserveRequest(1, plate, Sample.Now.AddDays(1), 3));

        Assert.False(result.IsSuccess);
        Assert.Equal("Номер автомобіля не вказано.", result.Error);
    }

    [Fact]
    public void Reserve_UnknownCustomer_Fails()
    {
        var result = _service.Reserve(new ReserveRequest(99, "AA1001AA", Sample.Now.AddDays(1), 3));

        Assert.Equal("Клієнта з кодом 99 не знайдено.", result.Error);
    }

    [Fact]
    public void Reserve_UnknownCar_Fails()
    {
        var result = _service.Reserve(new ReserveRequest(1, "XX0000XX", Sample.Now.AddDays(1), 3));

        Assert.Equal("Автомобіля XX0000XX у парку немає.", result.Error);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(31)]
    public void Reserve_DaysOutOfRange_Fails(int days)
    {
        var request = new ReserveRequest(1, "AA1001AA", Sample.Now.AddDays(1), days);

        var result = _service.Reserve(request);

        Assert.False(result.IsSuccess);
        Assert.StartsWith("Тривалість оренди — від 1 до 30 діб", result.Error);
    }

    [Fact]
    public void Reserve_StartInPast_Fails()
    {
        var request = new ReserveRequest(1, "AA1001AA", Sample.Now.AddHours(-1), 3);

        var result = _service.Reserve(request);

        Assert.Equal("Початок оренди не може бути в минулому.", result.Error);
    }

    [Fact]
    public void Reserve_OverlapsOpenRental_FailsAndSavesNothing()
    {
        _rentals.Add(Sample.Reserved(1001, "AA1001AA", startInDays: 1, days: 3));

        var result = _service.Reserve(new ReserveRequest(2, "AA1001AA", Sample.Now.AddDays(2), 3));

        Assert.False(result.IsSuccess);
        Assert.Contains("уже заброньовано або видано", result.Error);
        Assert.Equal(0, _rentals.SaveCount);
    }

    [Fact]
    public void Reserve_StartsWhenPreviousEnds_Succeeds()
    {
        _rentals.Add(Sample.Reserved(1001, "AA1001AA", startInDays: 1, days: 3));

        var result = _service.Reserve(new ReserveRequest(2, "AA1001AA", Sample.Now.AddDays(4), 2));

        Assert.True(result.IsSuccess);
        Assert.Equal(1002, result.Value!.Number);
    }

    [Fact]
    public void Cancel_Reserved_BecomesCancelled()
    {
        _rentals.Add(Sample.Reserved(1001, "AA1001AA"));

        var result = _service.Cancel(1001);

        Assert.Equal(RentalStatus.Cancelled, result.Value!.Status);
        Assert.Equal(1, _rentals.SaveCount);
    }

    [Fact]
    public void Cancel_UnknownNumber_Fails()
    {
        Assert.Equal("Договору № 7 не знайдено.", _service.Cancel(7).Error);
    }

    [Fact]
    public void Cancel_ActiveRental_ThrowsAndKeepsState()
    {
        _rentals.Add(Sample.Active(1001, "AA1001AA", Sample.Now.AddDays(-1)));

        Assert.Throws<InvalidRentalStateException>(() => _service.Cancel(1001));

        Assert.Equal(RentalStatus.Active, _rentals.Find(1001)!.Status);
        Assert.Equal(0, _rentals.SaveCount);
    }
}
