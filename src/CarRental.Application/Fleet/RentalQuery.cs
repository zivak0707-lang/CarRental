using CarRental.Domain.Rentals;

namespace CarRental.Application.Fleet;

/// <summary>Критерії пошуку оренд (UC-05); порожній критерій не обмежує вибірку.</summary>
/// <param name="Status">Стан документа.</param>
/// <param name="CustomerName">Частина прізвища або імені клієнта.</param>
/// <param name="Plate">Державний номер автомобіля.</param>
public sealed record RentalQuery(RentalStatus? Status, string? CustomerName, string? Plate);
