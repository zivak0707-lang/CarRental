using CarRental.Core.Domain;
using CarRental.Core.Rentals;

namespace CarRental.Tests;

public class RentalTotalCalculatorTests
{
    [Fact]
    public void Process_RegularCustomer_Returns1292()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 12, DailyRate = 100m },
            new RentalLine { Plate = "KA5678BB", Days = 1, DailyRate = 250m },
        };

        decimal total = RentalTotalCalculator.Process(
            1,
            "Іваненко",
            "i@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            null,
            RentalStatus.New,
            false,
            60m,
            out RentalStatus next);

        Assert.Equal(1292.70m, total);
        Assert.Equal(RentalStatus.Confirmed, next);
    }

    [Fact]
    public void Process_SmallRental_AddsDelivery()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 2, DailyRate = 200m },
        };

        decimal total = RentalTotalCalculator.Process(
            2,
            "Петренко",
            "p@ex.com",
            false,
            lines,
            new DateOnly(2026, 3, 10),
            "SALE10",
            RentalStatus.Confirmed,
            false,
            60m,
            out RentalStatus next);

        Assert.Equal(420m, total);
        Assert.Equal(RentalStatus.Active, next);
    }

    [Fact]
    public void Process_FreeRental_ReturnsOnlyDelivery()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 12, DailyRate = 0m },
        };

        decimal total = RentalTotalCalculator.Process(
            4,
            "Мельник",
            "m@ex.com",
            true,
            lines,
            new DateOnly(2026, 3, 10),
            null,
            RentalStatus.New,
            false,
            60m,
            out RentalStatus next);

        Assert.Equal(60m, total);
        Assert.Equal(RentalStatus.Confirmed, next);
    }

    [Fact]
    public void VolumeDiscountOf_LargeAmount_ReturnsTenPercent()
    {
        decimal discount = RentalTotalCalculator.VolumeDiscountOf(6000m);

        Assert.Equal(600m, discount);
    }

    [Fact]
    public void CouponDiscountOf_FixedCoupon_WorksOnlyAboveThreshold()
    {
        decimal above = RentalTotalCalculator.CouponDiscountOf(1500m, "MINUS200");
        decimal below = RentalTotalCalculator.CouponDiscountOf(800m, "MINUS200");

        Assert.Equal(200m, above);
        Assert.Equal(0m, below);
    }

    [Fact]
    public void NextStatusOf_NewWithPositiveTotal_ReturnsConfirmed()
    {
        RentalStatus next = RentalTotalCalculator.NextStatusOf(RentalStatus.New, 100m);

        Assert.Equal(RentalStatus.Confirmed, next);
    }
}
