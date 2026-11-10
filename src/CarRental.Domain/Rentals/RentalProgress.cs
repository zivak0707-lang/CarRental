namespace CarRental.Domain.Rentals;

/// <summary>Збережений хід оренди; використовується лише для відновлення зі сховища.</summary>
/// <param name="Status">Стан документа.</param>
/// <param name="PickedUpAt">Фактичний момент видачі.</param>
/// <param name="ReturnedAt">Фактичний момент повернення.</param>
/// <param name="Charge">Нарахування після закриття.</param>
public sealed record RentalProgress(
    RentalStatus Status,
    DateTimeOffset? PickedUpAt,
    DateTimeOffset? ReturnedAt,
    RentalCharge? Charge);
