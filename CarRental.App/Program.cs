using System.Text;
using CarRental.Core.Domain;
using CarRental.Core.Rentals;
using CarRental.Core.Reporting;

Console.OutputEncoding = Encoding.UTF8;

var booking = new Booking("B-1001", "Іваненко");
booking.AddLine("AA1234AA", 3, 250m);
booking.AddLine("KA5678KA", 12, 90m);
Console.WriteLine(booking.CalculateTotal(false));
Console.WriteLine(booking.CalculateTotal(true));
Console.WriteLine(booking.IsValid());
Console.WriteLine(booking.TryChangeStatus(1));
Console.Write(booking.BuildReport());

Console.WriteLine();
TotalRequest request = BuildSampleRequest();
TotalResult result = RentalTotalCalculator.Calculate(request);
string report = RentalReport.TextOf(request, result);
new RentalReportPrinter(Console.Out).Print(report);

static TotalRequest BuildSampleRequest()
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
