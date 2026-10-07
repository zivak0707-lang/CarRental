namespace CarRental.Core.Domain;

/// <summary>Рядок прокату: автомобіль, кількість діб і ціна за добу.</summary>
public sealed class RentalLine
{
    /// <summary>Державний номер автомобіля.</summary>
    public string Plate { get; init; } = string.Empty;

    /// <summary>Кількість діб прокату.</summary>
    public int Days { get; init; }

    /// <summary>Ставка прокату за добу, грн.</summary>
    public decimal DailyRate { get; init; }

    /// <summary>Вартість рядка: кількість діб, помножена на ставку.</summary>
    public decimal Amount => Days * DailyRate;
}
