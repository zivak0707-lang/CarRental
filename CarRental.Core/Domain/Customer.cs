namespace CarRental.Core.Domain;

/// <summary>Клієнт пункту прокату.</summary>
public sealed class Customer
{
    /// <summary>Ідентифікатор клієнта.</summary>
    public int Id { get; init; }

    /// <summary>Ім'я клієнта.</summary>
    public string Name { get; init; } = string.Empty;

    /// <summary>Ознака постійного клієнта.</summary>
    public bool IsRegular { get; init; }
}
