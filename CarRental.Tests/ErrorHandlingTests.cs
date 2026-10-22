using CarRental.Core.Domain;
using CarRental.Core.Errors;
using CarRental.Core.Notifications;
using CarRental.Core.Pricing;
using CarRental.Core.Services;
using CarRental.Core.Storage;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests;

public class ErrorHandlingTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);

    [Fact]
    public void Pay_UnknownReservation_ThrowsDomainRule()
    {
        ReservationService service = NewService();

        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.Pay(42, 100m));

        Assert.Equal("reservation.exists", ex.Rule);
    }

    [Fact]
    public void Pay_LessThanTotal_ThrowsDomainRuleWithAmounts()
    {
        ReservationService service = NewService();
        Reservation reservation = new(1, 100, _start);
        service.Place(reservation);
        service.AddLine(1, new ReservationDay("AA1234KI", _start, _start.AddDays(2), 250m));
        reservation.Confirm();

        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.Pay(1, 400m));

        Assert.Equal("reservation.amount", ex.Rule);
        Assert.Contains("500", ex.Message);
    }

    private static ReservationService NewService() =>
        new(
            new InMemoryReservationRepository(),
            new StandardPricingPolicy(),
                 new TextWriterNotifier(TextWriter.Null),
                 NullLogger<ReservationService>.Instance);
}
