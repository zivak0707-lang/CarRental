using CarRental.Core.Domain;

namespace CarRental.Core.Rentals;

/// <summary>Результат розрахунку: підсумок і стан прокату після розрахунку.</summary>
/// <param name="Total">Підсумок прокату, грн.</param>
/// <param name="Status">Стан прокату після розрахунку.</param>
public sealed record TotalResult(decimal Total, RentalStatus Status);
