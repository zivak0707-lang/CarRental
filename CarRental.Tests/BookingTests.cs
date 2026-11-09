using CarRental.Core.Domain;

namespace CarRental.Tests;

public class BookingTests
{
    private static readonly DateTimeOffset _createdAt = new(2026, 11, 2, 9, 0, 0, TimeSpan.Zero);

    [Theory]
    [InlineData(1000, true, 1200)]
    [InlineData(1000.01, true, 1080.01)]
    [InlineData(5000, false, 6000)]
    [InlineData(5000.01, false, 5100.01)]
    [InlineData(6000, true, 6480)]
    public void CalculateTotal_AmountBoundaries_MatchesRules(
        double amount, bool isRegular, double expected)
    {
        // Arrange
        Booking booking = NewBooking(1, (decimal)amount);

        // Act
        decimal actual = booking.CalculateTotal(isRegular);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(10, 100, 1200)]
    [InlineData(11, 100, 1200)]
    [InlineData(11, 1, 0)]
    public void CalculateTotal_LineCount_AppliesBulkDiscount(
        int lines, double dailyRate, double expected)
    {
        // Arrange
        Booking booking = NewBooking(lines, (decimal)dailyRate);

        // Act
        decimal actual = booking.CalculateTotal(false);

        // Assert
        Assert.Equal((decimal)expected, actual);
    }

    [Theory]
    [InlineData(Booking.StatusConfirmed, true)]
    [InlineData(Booking.StatusCancelled, true)]
    [InlineData(Booking.StatusIssued, false)]
    public void TryChangeStatus_FromNew_FollowsTransitions(int target, bool expected)
    {
        // Arrange
        Booking booking = NewBooking(1, 100m);

        // Act
        bool actual = booking.TryChangeStatus(target);

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void TryChangeStatus_ConfirmedToIssued_ChangesStatus()
    {
        // Arrange
        Booking booking = NewBooking(1, 100m);
        booking.TryChangeStatus(Booking.StatusConfirmed);

        // Act
        booking.TryChangeStatus(Booking.StatusIssued);

        // Assert
        Assert.Equal(Booking.StatusIssued, booking.Status);
    }

    [Theory]
    [InlineData("B-1", "Петренко", 1, true)]
    [InlineData("", "Петренко", 1, false)]
    [InlineData("B-1", "Ян", 1, false)]
    [InlineData("B-1", "Петренко", 0, false)]
    public void IsValid_Fields_MatchesRules(string id, string name, int lines, bool expected)
    {
        // Arrange
        Booking booking = new(id, name, _createdAt);
        AddLines(booking, lines, 100m);

        // Act
        bool actual = booking.IsValid();

        // Assert
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void BuildReport_OneLine_ContainsPlateAndTotalLabel()
    {
        // Arrange
        Booking booking = NewBooking(1, 100m);

        // Act
        string report = booking.BuildReport();

        // Assert
        Assert.Contains("Авто: AA1234KI; діб: 1", report);
        Assert.Contains("Разом: ", report);
    }

    [Fact]
    public void FindById_ExistingAndMissing_ReturnsBookingOrNull()
    {
        // Arrange
        Booking booking = NewBooking(1, 100m);
        List<Booking> bookings = new() { booking };

        // Act
        Booking? found = Booking.FindById(bookings, "B-1");
        Booking? missing = Booking.FindById(bookings, "B-2");

        // Assert
        Assert.Same(booking, found);
        Assert.Null(missing);
    }

    private static Booking NewBooking(int lines, decimal dailyRate)
    {
        Booking booking = new("B-1", "Петренко", _createdAt);
        AddLines(booking, lines, dailyRate);
        return booking;
    }

    private static void AddLines(Booking booking, int lines, decimal dailyRate)
    {
        for (int i = 0; i < lines; i++)
        {
            booking.AddLine("AA1234KI", 1, dailyRate);
        }
    }
}
