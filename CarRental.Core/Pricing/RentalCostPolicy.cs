namespace CarRental.Core.Pricing;

/// <summary>Вартість доби прокату: тариф вихідного дня та багатоденна знижка.</summary>
public static class RentalCostPolicy
{
    /// <summary>Надбавка тарифу вихідного дня, частка від ставки.</summary>
    public const decimal WeekendRate = 0.20m;

    /// <summary>Знижка багатоденного прокату, частка від ставки.</summary>
    public const decimal MultiDayRate = 0.10m;

    /// <summary>Кількість діб, від якої діє багатоденна знижка.</summary>
    public const int MultiDayFrom = 5;

    /// <summary>Рахує вартість однієї доби прокату.</summary>
    /// <param name="dailyRate">Базова ставка за добу, грн, більша за нуль.</param>
    /// <param name="days">Тривалість прокату, діб, не менше однієї.</param>
    /// <param name="isWeekend">Чи припадає доба на вихідний.</param>
    /// <returns>Вартість доби, грн, округлена до копійок.</returns>
    public static decimal DailyCostFor(decimal dailyRate, int days, bool isWeekend)
    {
        if (dailyRate <= 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(dailyRate),
                "Ставка має бути більшою за нуль.");
        }

        if (days < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(days),
                "Тривалість прокату — щонайменше одна доба.");
        }

        decimal factor = 1m;
        if (isWeekend)
        {
            factor += WeekendRate;
        }

        if (days >= MultiDayFrom)
        {
            factor -= MultiDayRate;
        }

        return decimal.Round(dailyRate * factor, 2);
    }
}
