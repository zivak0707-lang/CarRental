namespace CarRental.Domain.Rentals;

/// <summary>Нарахування при закритті оренди.</summary>
/// <param name="BillableDays">Оплачувані доби з округленням угору.</param>
/// <param name="RentCost">Вартість діб, грн.</param>
/// <param name="LateFee">Штраф за пізнє повернення, грн.</param>
public sealed record RentalCharge(int BillableDays, decimal RentCost, decimal LateFee)
{
    /// <summary>Сума до сплати, грн.</summary>
    public decimal Total => RentCost + LateFee;
}
