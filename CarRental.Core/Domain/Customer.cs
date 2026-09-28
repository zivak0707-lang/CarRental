namespace CarRental.Core.Domain;

/// <summary>Клієнт пункту прокату.</summary>
public sealed class Customer
{
    public int Id { get; init; }

    public string Name { get; init; } = string.Empty;

    public bool IsRegular { get; init; }
}