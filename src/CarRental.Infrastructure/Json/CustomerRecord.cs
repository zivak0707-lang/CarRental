namespace CarRental.Infrastructure.Json;

/// <summary>Запис файлу customers.json.</summary>
/// <param name="Id">Код клієнта.</param>
/// <param name="FullName">Прізвище та ім'я.</param>
/// <param name="LicenseNumber">Номер посвідчення водія.</param>
internal sealed record CustomerRecord(int Id, string FullName, string LicenseNumber);
