using CarRental.Domain.Cars;

namespace CarRental.Domain.Tariffs;

/// <summary>Тариф класу автомобілів: ціна однієї доби оренди.</summary>
public sealed class Tariff
{
    /// <summary>Створює тариф.</summary>
    /// <param name="carClass">Клас, до якого застосовується тариф.</param>
    /// <param name="dailyRate">Ціна доби, грн; більша за нуль.</param>
    public Tariff(CarClass carClass, decimal dailyRate)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dailyRate);
        Class = carClass;
        DailyRate = dailyRate;
    }

    /// <summary>Клас автомобілів.</summary>
    public CarClass Class { get; }

    /// <summary>Ціна доби, грн.</summary>
    public decimal DailyRate { get; }
}
