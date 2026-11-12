using CarRental.Application.Fleet;
using CarRental.Domain.Cars;
using CarRental.Domain.Rentals;
using CarRental.Tests.Doubles;
using Microsoft.Extensions.Logging.Abstractions;

namespace CarRental.Tests.Application;

/// <summary>UC-05 «Знайти оренди» і UC-06 «Зайнятість автопарку».</summary>
public class FleetQueryServiceTests
{
    private readonly FakeRentalRepository _rentals = new();
    private readonly FleetQueryService _service;

    public FleetQueryServiceTests()
    {
        _service = new FleetQueryService(
            _rentals, new FakeCatalog(), NullLogger<FleetQueryService>.Instance);
        _rentals.Add(Sample.Active(1003, "AA2001BB", Sample.Now.AddDays(-1)));
        _rentals.Add(Sample.Reserved(1001, "AA1001AA", startInDays: 1));
        _rentals.Add(Sample.Reserved(1002, "AA1002AA", startInDays: 5));
    }

    [Fact]
    public void Find_NothingMatches_ReturnsEmpty()
    {
        Assert.Empty(_service.Find(new RentalQuery(RentalStatus.Closed, null, null)));
    }

    [Fact]
    public void Find_ByPlateInLowerCase_ReturnsOne()
    {
        var found = _service.Find(new RentalQuery(null, null, "aa2001"));

        Assert.Equal(1003, Assert.Single(found).Number);
    }

    [Fact]
    public void Find_ByStatusAndName_ReturnsSeveralInNumberOrder()
    {
        var found = _service.Find(new RentalQuery(RentalStatus.Reserved, "петренко", null));

        Assert.Equal("1001,1002", string.Join(",", found.Select(r => r.Number)));
    }

    [Fact]
    public void Occupancy_Day_CountsRentedReservedAndFreePerClass()
    {
        var report = _service.Occupancy(DateOnly.FromDateTime(Sample.Now.AddDays(1).DateTime));

        OccupancyRow economy = report.Rows.Single(r => r.Class == CarClass.Economy);
        OccupancyRow suv = report.Rows.Single(r => r.Class == CarClass.Suv);
        Assert.Equal(new OccupancyRow(CarClass.Economy, 2, 0, 1), economy);
        Assert.Equal((1, 50.0m), (economy.Free, economy.Percent));
        Assert.Equal((1, 0, 100.0m), (suv.Rented, suv.Free, suv.Percent));
        Assert.Equal(new OccupancyRow(null, 3, 1, 1), report.Total);
    }

    [Fact]
    public void Occupancy_ClassWithoutCars_HasZeroPercent()
    {
        var report = _service.Occupancy(DateOnly.FromDateTime(Sample.Now.DateTime));

        OccupancyRow premium = report.Rows.Single(r => r.Class == CarClass.Premium);
        Assert.Equal((0, 0m), (premium.Total, premium.Percent));
        Assert.Equal(4, report.Rows.Count);
    }
}
