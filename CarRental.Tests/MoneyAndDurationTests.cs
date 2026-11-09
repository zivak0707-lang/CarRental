using CarRental.Core.Domain;
using CarRental.Core.Notifications;
using CarRental.Core.Pricing;
using CarRental.Core.Services;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests;

public class MoneyAndDurationTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);
    private static readonly DateTimeOffset _pickedUpAt = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(2.125, 2.13)]
    [InlineData(2.135, 2.14)]
    [InlineData(2.124, 2.12)]
    public void RoundToKopecks_MidpointValues_RoundsAwayFromZero(double amount, double expected)
    {
        // Act
        decimal actual = Money.RoundToKopecks((decimal)amount);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(86400, 1)]
    [InlineData(86401, 2)]
    [InlineData(172800, 2)]
    public void BillableDays_RentalLength_RoundsUpToWholeDays(int seconds, int expected)
    {
        // Arrange
        DateTimeOffset returnedAt = _pickedUpAt.AddSeconds(seconds);

        // Act
        int actual = RentalDuration.BillableDays(_pickedUpAt, returnedAt);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-60)]
    public void BillableDays_ReturnNotAfterPickUp_ThrowsOutOfRange(int seconds)
    {
        // Arrange
        DateTimeOffset returnedAt = _pickedUpAt.AddSeconds(seconds);

        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => RentalDuration.BillableDays(_pickedUpAt, returnedAt));

        // Assert
        Assert.Equal("returnedAt", ex.ParamName);
    }

    [Fact]
    public void TotalOf_ThreeSmallLines_RoundsOnceOnDocumentTotal()
    {
        // Arrange
        ReservationService service = new(
            new FakeReservationRepository(),
            new VipPricingPolicy(),
            new TextWriterNotifier(TextWriter.Null),
            NullLogger<ReservationService>.Instance);
        Reservation reservation = new(1, 5, _start);
        for (int i = 0; i < 3; i++)
        {
            reservation.AddLine(new ReservationDay("AA1234KI", _start, _start.AddDays(1), 0.05m));
        }

        // Act
        decimal total = service.TotalOf(reservation);

        // Assert
        Assert.Equal(0.14m, total);
    }
}
