using CarRental.Core.Domain;
using CarRental.Core.Rentals;

namespace CarRental.Tests;

public class RentalTotalCalculatorTests
{
    [Fact]
    public void Calculate_RegularCustomer_Returns1292()
    {
        TotalRequest request = SampleRequests.RegularCustomer();

        TotalResult result = RentalTotalCalculator.Calculate(request);

        Assert.Equal(1292.70m, result.Total);
        Assert.Equal(RentalStatus.Confirmed, result.Status);
    }

    [Fact]
    public void Calculate_SmallRental_AddsDelivery()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 2, DailyRate = 200m },
        };
        TotalRequest request = new(
            RentalId: 2,
            Customer: new CustomerInfo("Петренко", "p@ex.com", false),
            Lines: lines,
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: "SALE10",
            Status: RentalStatus.Confirmed,
            DeliveryPrice: 60m);

        TotalResult result = RentalTotalCalculator.Calculate(request);

        Assert.Equal(420m, result.Total);
        Assert.Equal(RentalStatus.Active, result.Status);
    }

    [Fact]
    public void Calculate_FreeRental_ReturnsOnlyDelivery()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 12, DailyRate = 0m },
        };
        TotalRequest request = new(
            RentalId: 4,
            Customer: new CustomerInfo("Мельник", "m@ex.com", true),
            Lines: lines,
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: RentalStatus.New,
            DeliveryPrice: 60m);

        TotalResult result = RentalTotalCalculator.Calculate(request);

        Assert.Equal(60m, result.Total);
        Assert.Equal(RentalStatus.Confirmed, result.Status);
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
