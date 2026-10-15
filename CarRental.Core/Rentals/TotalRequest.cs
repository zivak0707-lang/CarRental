using CarRental.Core.Domain;

namespace CarRental.Core.Rentals;

/// <summary>Запит на розрахунок підсумку прокату: усі вхідні дані в одному об'єкті.</summary>
/// <param name="RentalId">Номер прокату.</param>
/// <param name="Customer">Дані клієнта.</param>
/// <param name="Lines">Рядки прокату.</param>
/// <param name="CreatedAt">Дата створення прокату.</param>
/// <param name="CouponCode">Код купона або null, якщо купона немає.</param>
/// <param name="Status">Поточний стан прокату.</param>
/// <param name="DeliveryPrice">Вартість подачі автомобіля, грн.</param>
public sealed record TotalRequest(
    int RentalId,
    CustomerInfo Customer,
    IReadOnlyList<RentalLine> Lines,
    DateOnly CreatedAt,
    string? CouponCode,
    RentalStatus Status,
    decimal DeliveryPrice);
