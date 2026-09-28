namespace CarRental.Core.Domain;

/// <summary>Автомобіль у парку прокату.</summary>
public sealed class Car
{
    public string Plate { get; init; } = string.Empty;

    public string Model { get; init; } = string.Empty;

    public decimal DailyRate { get; init; }

    public bool IsAvailable { get; init; }
}
