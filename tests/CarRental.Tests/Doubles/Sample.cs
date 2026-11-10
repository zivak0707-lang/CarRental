using CarRental.Domain.Rentals;

namespace CarRental.Tests.Doubles;

/// <summary>Спільні тестові дані: «зараз» — 10.11.2026 10:00 за Києвом.</summary>
public static class Sample
{
    public static readonly DateTimeOffset Now = new(2026, 11, 10, 10, 0, 0, TimeSpan.FromHours(2));

    public static Rental Reserved(int number, string plate, int startInDays = 1, int days = 3)
    {
        DateTimeOffset start = Now.AddDays(startInDays);
        var terms = new RentalTerms(1, plate, new RentalPeriod(start, start.AddDays(days)), 900m);
        return new Rental(number, terms);
    }

    public static Rental Active(int number, string plate, DateTimeOffset pickedUpAt, int days = 3)
    {
        var period = new RentalPeriod(pickedUpAt, pickedUpAt.AddDays(days));
        var terms = new RentalTerms(2, plate, period, 900m);
        var rental = new Rental(number, terms);
        rental.PickUp(pickedUpAt);
        return rental;
    }
}
