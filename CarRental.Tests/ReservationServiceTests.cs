using CarRental.Core.Abstractions;
using CarRental.Core.Domain;
using CarRental.Core.Errors;
using CarRental.Core.Notifications;
using CarRental.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests;

public class ReservationServiceTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);

    [Fact]
    public void TotalOf_TwoLines_SumsPricesFromPolicy()
    {
        // Arrange
        ReservationService service = NewService(
            new FakeReservationRepository(),
            new StubPricingPolicy(25m));
        Reservation reservation = new(7, 1, _start);
        reservation.AddLine(new ReservationDay("AA1234KI", _start, _start.AddDays(2), 10m));
        reservation.AddLine(new ReservationDay("KA5678BB", _start, _start.AddDays(1), 30m));

        // Act
        decimal actual = service.TotalOf(reservation);

        // Assert
        Assert.Equal(50m, actual);
    }

    [Fact]
    public void Place_Reservation_StoresAndAddsOnce()
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository, new StubPricingPolicy(1m));
        Reservation reservation = new(1, 5, _start);

        // Act
        service.Place(reservation);

        // Assert
        Assert.NotNull(repository.GetById(1));
        Assert.Equal(1, repository.AddCalls);
    }

    [Fact]
    public void Load_UnknownId_ThrowsDomainRule()
    {
        // Arrange
        ReservationService service = NewService(
            new FakeReservationRepository(),
            new StubPricingPolicy(1m));

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(() => service.Load(404));

        // Assert
        Assert.Equal("reservation.exists", ex.Rule);
    }

    private static ReservationService NewService(
        IReservationRepository repository,
        IPricingPolicy pricing) =>
        new(
            repository,
            pricing,
            new TextWriterNotifier(TextWriter.Null),
            NullLogger<ReservationService>.Instance);
}
