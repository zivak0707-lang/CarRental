namespace CarRental.Infrastructure.Json;

/// <summary>Запис файлу cars.json.</summary>
/// <param name="Plate">Державний номер.</param>
/// <param name="Model">Марка й модель.</param>
/// <param name="Class">Назва класу з переліку CarClass.</param>
internal sealed record CarRecord(string Plate, string Model, string Class);
