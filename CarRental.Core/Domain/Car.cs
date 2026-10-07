namespace CarRental.Core.Domain;

/// <summary>Автомобіль у парку прокату.</summary>
public sealed class Car
{
    /// <summary>Державний номер автомобіля.</summary>
    public string Plate { get; init; } = string.Empty;

    /// <summary>Модель автомобіля.</summary>
    public string Model { get; init; } = string.Empty;

    /// <summary>Ставка прокату за добу, грн.</summary>
    public decimal DailyRate { get; init; }

    /// <summary>Ознака того, що автомобіль доступний для прокату.</summary>
    public bool IsAvailable { get; init; }
}
