using CarRental.Core.Domain;
using CarRental.Core.Errors;
using CarRental.Core.Pricing;
using CarRental.Core.Reporting;
using CarRental.Core.Storage;

namespace CarRental.Tests;

public class SmallTypesTests
{
    private static readonly DateOnly _start = new(2026, 11, 2);

    [Fact]
    public void Renter_ValidData_KeepsFields()
    {
        // Act
        Renter renter = new(1, "Іваненко Петро", "petro@example.com") { IsRegular = true };

        // Assert
        Assert.Equal("petro@example.com", renter.Email);
        Assert.True(renter.IsRegular);
    }

    [Fact]
    public void Renter_EmailWithoutAt_ThrowsWithParamName()
    {
        // Act
        ArgumentException ex = Assert.Throws<ArgumentException>(
            () => new Renter(1, "Іваненко Петро", "petro.example.com"));

        // Assert
        Assert.Equal("email", ex.ParamName);
    }

    [Fact]
    public void Rental_TwoLines_TotalIsSumOfAmounts()
    {
        // Arrange
        Rental rental = new() { Id = 1, CustomerId = 2 };
        rental.AddLine(new RentalLine { Plate = "AA1234KI", Days = 2, DailyRate = 100m });
        rental.AddLine(new RentalLine { Plate = "KA5678BB", Days = 1, DailyRate = 50m });

        // Act
        decimal total = rental.Total();

        // Assert
        Assert.Equal(250m, total);
    }

    [Fact]
    public void Result_OkAndFail_CarryValueOrError()
    {
        // Act
        Result<int> ok = Result.Ok(5);
        Result<int> fail = Result.Fail<int>("помилка");

        // Assert
        Assert.True(ok.IsSuccess);
        Assert.Equal(5, ok.Value);
        Assert.False(fail.IsSuccess);
        Assert.Equal("помилка", fail.Error);
    }

    [Fact]
    public void VipPricing_TwoDays_TakesTenPercentOff()
    {
        // Arrange
        VipPricingPolicy policy = new();
        ReservationDay line = new("AA1234KI", _start, _start.AddDays(2), 100m);

        // Act
        decimal price = policy.PriceOf(line);

        // Assert
        Assert.Equal(180m, price);
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(0.51)]
    public void DiscountPricing_RateOutOfRange_ThrowsWithParamName(double rate)
    {
        // Act
        ArgumentOutOfRangeException ex = Assert.Throws<ArgumentOutOfRangeException>(
            () => new DiscountPricingPolicy((decimal)rate));

        // Assert
        Assert.Equal("rate", ex.ParamName);
    }

    [Fact]
    public void BuildCsv_OneReservation_WritesInvariantRow()
    {
        // Arrange
        Reservation reservation = new(1, 5, _start);
        reservation.AddLine(new ReservationDay("AA1234KI", _start, _start.AddDays(2), 250m));

        // Act
        string csv = ReservationCsvReport.BuildCsv(new[] { reservation });

        // Assert
        Assert.Contains("1;Draft;2;500.00", csv);
    }

    [Fact]
    public void InMemoryRepository_SameIdTwice_ThrowsInvalidOperation()
    {
        // Arrange
        InMemoryReservationRepository repository = new();
        repository.Add(new Reservation(1, 5, _start));

        // Act
        void Act() => repository.Add(new Reservation(1, 6, _start));

        // Assert
        Assert.Throws<InvalidOperationException>(Act);
        Assert.Single(repository.GetAll());
    }
}
