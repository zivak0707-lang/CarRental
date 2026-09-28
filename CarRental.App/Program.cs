using System.Globalization;
using CarRental.Core.Domain;

Rental rental = new()
{
    Id = 1,
    CustomerId = 10,
    CreatedAt = DateTimeOffset.Now,
};

rental.AddLine(new RentalLine
{
    Plate = "AA1234KI",
    Days = 3,
    DailyRate = 1200.00m,
});
rental.AddLine(new RentalLine
{
    Plate = "KA5678BB",
    Days = 2,
    DailyRate = 950.50m,
});

string total = rental.Total()
    .ToString("F2", CultureInfo.InvariantCulture);

Console.WriteLine($"Прокат #{rental.Id}");
Console.WriteLine($"Стан: {rental.Status}");
Console.WriteLine($"Позицій: {rental.Lines.Count}");
Console.WriteLine($"Сума: {total}");
