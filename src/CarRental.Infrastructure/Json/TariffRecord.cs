namespace CarRental.Infrastructure.Json;

/// <summary>Запис файлу tariffs.json.</summary>
/// <param name="Class">Назва класу з переліку CarClass.</param>
/// <param name="DailyRate">Ціна доби, грн.</param>
internal sealed record TariffRecord(string Class, decimal DailyRate);
