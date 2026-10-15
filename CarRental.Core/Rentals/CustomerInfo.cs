namespace CarRental.Core.Rentals;

/// <summary>Дані клієнта, потрібні для розрахунку підсумку прокату.</summary>
/// <param name="Name">Ім'я клієнта.</param>
/// <param name="Email">Пошта клієнта.</param>
/// <param name="IsRegular">Ознака постійного клієнта.</param>
public sealed record CustomerInfo(string Name, string Email, bool IsRegular);
