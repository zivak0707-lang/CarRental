namespace CarRental.Domain.Customers;

/// <summary>Клієнт прокату, який має право керувати автомобілем.</summary>
public sealed class Customer
{
    /// <summary>Створює клієнта.</summary>
    /// <param name="id">Код клієнта, більший за нуль.</param>
    /// <param name="fullName">Прізвище та ім'я.</param>
    /// <param name="licenseNumber">Номер посвідчення водія (умовний).</param>
    public Customer(int id, string fullName, string licenseNumber)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(fullName);
        ArgumentException.ThrowIfNullOrWhiteSpace(licenseNumber);
        Id = id;
        FullName = fullName;
        LicenseNumber = licenseNumber;
    }

    /// <summary>Код клієнта.</summary>
    public int Id { get; }

    /// <summary>Прізвище та ім'я.</summary>
    public string FullName { get; }

    /// <summary>Номер посвідчення водія.</summary>
    public string LicenseNumber { get; }
}
