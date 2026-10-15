using CarRental.Core.Domain;
using CarRental.Core.Rentals;

namespace CarRental.Tests;

internal static class SampleRequests
{
    public static TotalRequest RegularCustomer()
    {
        List<RentalLine> lines = new()
        {
            new RentalLine { Plate = "AA1234KI", Days = 12, DailyRate = 100m },
            new RentalLine { Plate = "KA5678BB", Days = 1, DailyRate = 250m },
        };

        return new TotalRequest(
            RentalId: 1,
            Customer: new CustomerInfo("Іваненко", "i@ex.com", true),
            Lines: lines,
            CreatedAt: new DateOnly(2026, 3, 10),
            CouponCode: null,
            Status: RentalStatus.New,
            DeliveryPrice: 60m);
    }
}
