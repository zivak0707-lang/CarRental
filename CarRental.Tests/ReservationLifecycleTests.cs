using CarRental.Core.Domain;

namespace CarRental.Tests;

public class ReservationLifecycleTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);

    [Fact]
    public void Confirm_NoLines_ThrowsInvalidOperation()
    {
        // Arrange
        Reservation reservation = new(1, 5, _start);

        // Act
        void Act() => reservation.Confirm();

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void PickUp_DraftReservation_ThrowsInvalidOperation()
    {
        // Arrange
        Reservation reservation = new(1, 5, _start);

        // Act
        void Act() => reservation.PickUp();

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    [Fact]
    public void Close_ActiveReservation_SetsClosed()
    {
        // Arrange
        Reservation reservation = ActiveReservation();

        // Act
        reservation.Close();

        // Assert
        Assert.Equal(ReservationStatus.Closed, reservation.Status);
    }

    [Fact]
    public void Archive_ClosedReservation_KeepsIdStatusAndLines()
    {
        // Arrange
        Reservation reservation = ActiveReservation();
        reservation.Close();

        // Act
        ArchivedReservation archived = new(reservation, _start.AddDays(5));

        // Assert
        Assert.Equal(1, archived.Id);
        Assert.Equal(ReservationStatus.Closed, archived.Status);
        Assert.Equal(_start.AddDays(5), archived.ArchivedAt);
        Assert.Single(archived.Lines);
    }

    [Fact]
    public void Archive_ActiveReservation_ThrowsInvalidOperation()
    {
        // Arrange
        Reservation reservation = ActiveReservation();

        // Act
        void Act() => _ = new ArchivedReservation(reservation, _start);

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
    }

    private static Reservation ActiveReservation()
    {
        Reservation reservation = new(1, 5, _start);
        reservation.AddLine(new ReservationDay("AA1234KI", _start, _start.AddDays(1), 100m));
        reservation.Confirm();
        reservation.PickUp();
        return reservation;
    }
}
