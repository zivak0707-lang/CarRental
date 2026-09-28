namespace CarRental.Core.Domain;

/// <summary>Рядок прокату: автомобіль, кількість діб і ціна за добу.</summary>
public sealed class RentalLine
{
    public string Plate { get; init; } = string.Empty;

    public int Days { get; init; }

    public decimal DailyRate { get; init; }

    public decimal Amount => Days * DailyRate;
}