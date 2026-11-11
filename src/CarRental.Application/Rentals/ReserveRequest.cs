namespace CarRental.Application.Rentals;

/// <summary>Запит на бронювання (UC-01); перевіряється барикадою ReservationService.</summary>
/// <param name="CustomerId">Код клієнта.</param>
/// <param name="Plate">Державний номер автомобіля.</param>
/// <param name="Start">Запланований момент видачі.</param>
/// <param name="Days">Кількість діб оренди.</param>
public sealed record ReserveRequest(int CustomerId, string? Plate, DateTimeOffset Start, int Days);
