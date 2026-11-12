using CarRental.Domain.Rentals;

namespace CarRental.Infrastructure.Json;

/// <summary>Перетворення між записом rentals.json і документом оренди.</summary>
internal static class RentalMapper
{
    /// <summary>Відновлює оренду із запису файлу.</summary>
    /// <param name="r">Запис.</param>
    /// <returns>Оренда в збереженому стані.</returns>
    public static Rental ToDomain(RentalRecord r)
    {
        var period = new RentalPeriod(r.Start, r.End);
        var terms = new RentalTerms(r.CustomerId, r.CarPlate, period, r.DailyRate);
        RentalCharge? charge = r.BillableDays is int days
            ? new RentalCharge(days, r.RentCost ?? 0m, r.LateFee ?? 0m)
            : null;
        var status = Enum.Parse<RentalStatus>(r.Status);
        var progress = new RentalProgress(status, r.PickedUpAt, r.ReturnedAt, charge);
        return Rental.Restore(r.Number, terms, progress);
    }

    /// <summary>Перетворює оренду на запис файлу.</summary>
    /// <param name="rental">Оренда.</param>
    /// <returns>Запис для серіалізації.</returns>
    public static RentalRecord ToRecord(Rental rental) => new()
    {
        Number = rental.Number,
        CustomerId = rental.Terms.CustomerId,
        CarPlate = rental.Terms.CarPlate,
        Start = rental.Terms.Period.Start,
        End = rental.Terms.Period.End,
        DailyRate = rental.Terms.DailyRate,
        Status = rental.Status.ToString(),
        PickedUpAt = rental.PickedUpAt,
        ReturnedAt = rental.ReturnedAt,
        BillableDays = rental.Charge?.BillableDays,
        RentCost = rental.Charge?.RentCost,
        LateFee = rental.Charge?.LateFee,
    };
}
