using CarRental.Core.Domain;
using CarRental.Core.Errors;
using CarRental.Core.Notifications;
using CarRental.Core.Pricing;
using CarRental.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests;

public class ReservationServiceBranchTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);

    [Fact]
    public void Pay_DraftReservation_ThrowsPayable()
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository);
        repository.Add(new Reservation(1, 5, _start));

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.Pay(1, 100m));

        // Assert
        Assert.Equal("reservation.payable", ex.Rule);
    }

    [Theory]
    [InlineData(100)]
    [InlineData(150)]
    public void Pay_EnoughMoney_IssuesCar(double amount)
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository);
        Reservation reservation = ConfirmedReservation(repository);

        // Act
        service.Pay(reservation.Id, (decimal)amount);

        // Assert
        Assert.Equal(ReservationStatus.Active, reservation.Status);
    }

    [Fact]
    public void AddLine_DraftReservation_AddsLine()
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository);
        Reservation reservation = new(1, 5, _start);
        repository.Add(reservation);

        // Act
        service.AddLine(1, NewLine());

        // Assert
        Assert.Equal(NewLine(), reservation.Lines[0]);
    }

    [Fact]
    public void AddLine_ConfirmedReservation_ThrowsEditable()
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository);
        Reservation reservation = ConfirmedReservation(repository);

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.AddLine(reservation.Id, NewLine()));

        // Assert
        Assert.Equal("reservation.editable", ex.Rule);
    }

    [Fact]
    public void Cancel_DraftReservation_SetsCancelled()
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository);
        Reservation reservation = new(1, 5, _start);
        repository.Add(reservation);

        // Act
        service.Cancel(1, "клієнт передумав");

        // Assert
        Assert.Equal(ReservationStatus.Cancelled, reservation.Status);
    }

    [Fact]
    public void Cancel_IssuedCar_ThrowsCancellableWithCause()
    {
        // Arrange
        FakeReservationRepository repository = new();
        ReservationService service = NewService(repository);
        Reservation reservation = ConfirmedReservation(repository);
        reservation.PickUp();

        // Act
        DomainRuleException ex = Assert.Throws<DomainRuleException>(
            () => service.Cancel(reservation.Id, "запізно"));

        // Assert
        Assert.Equal("reservation.cancellable", ex.Rule);
        Assert.IsType<InvalidOperationException>(ex.InnerException);
    }

    private static ReservationDay NewLine() =>
        new("AA1234KI", _start, _start.AddDays(1), 100m);

    private static Reservation ConfirmedReservation(FakeReservationRepository repository)
    {
        Reservation reservation = new(1, 5, _start);
        reservation.AddLine(NewLine());
        reservation.Confirm();
        repository.Add(reservation);
        return reservation;
    }

    private static ReservationService NewService(FakeReservationRepository repository) =>
        new(
            repository,
            new StandardPricingPolicy(),
            new TextWriterNotifier(TextWriter.Null),
            NullLogger<ReservationService>.Instance);
}
