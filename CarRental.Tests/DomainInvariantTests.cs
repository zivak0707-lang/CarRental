using CarRental.Core.Domain;
using CarRental.Core.Notifications;
using CarRental.Core.Pricing;
using CarRental.Core.Services;
using CarRental.Core.Storage;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests;

public class DomainInvariantTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);

    [Fact]
    public void ReservationDay_WithEndNotAfterStart_Throws()
    {
        Assert.Throws<ArgumentOutOfRangeException>(
            () => new ReservationDay("AA1234KI", _start, _start, 100m));
    }

    [Fact]
    public void Reservation_AfterConfirm_RejectsNewLines()
    {
        Reservation reservation = NewReservation(1);
        reservation.AddLine(new ReservationDay("AA1234KI", _start, _start.AddDays(1), 100m));
        reservation.Confirm();

        Assert.Throws<InvalidOperationException>(
            () => reservation.AddLine(
                new ReservationDay("KA5678BB", _start, _start.AddDays(1), 50m)));
    }

    [Fact]
    public void TotalOf_WithTenPercentDiscount_Returns180()
    {
        ReservationService service = new(
            new InMemoryReservationRepository(),
            new DiscountPricingPolicy(0.10m),
                 new TextWriterNotifier(TextWriter.Null),
                 NullLogger<ReservationService>.Instance);
        Reservation reservation = NewReservation(2);
        reservation.AddLine(new ReservationDay("AA1234KI", _start, _start.AddDays(2), 100m));

        Assert.Equal(180m, service.TotalOf(reservation));
    }

    private static Reservation NewReservation(int id) =>
        new(id, 100, DateOnly.FromDateTime(DateTime.Today));
}
