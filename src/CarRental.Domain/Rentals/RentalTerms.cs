namespace CarRental.Domain.Rentals;

/// <summary>Умови договору оренди; тариф фіксується в момент бронювання.</summary>
/// <param name="CustomerId">Код клієнта.</param>
/// <param name="CarPlate">Державний номер автомобіля.</param>
/// <param name="Period">Запланований період.</param>
/// <param name="DailyRate">Ціна доби за договором, грн.</param>
public sealed record RentalTerms(
    int CustomerId, string CarPlate, RentalPeriod Period, decimal DailyRate);
