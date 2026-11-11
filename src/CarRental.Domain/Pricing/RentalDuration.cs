namespace CarRental.Domain.Pricing;

/// <summary>Перетворення тривалості на оплачувані доби з округленням угору.</summary>
public static class RentalDuration
{
    /// <summary>Кількість розпочатих діб: кожна розпочата доба оплачується повністю.</summary>
    /// <param name="duration">Тривалість, більша за нуль.</param>
    /// <returns>Кількість діб, щонайменше 1.</returns>
    public static int StartedDays(TimeSpan duration)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(duration, TimeSpan.Zero);

        // Рахуємо в тіках, а не через TotalDays: double дає похибку саме на межі доби
        long fullDays = duration.Ticks / TimeSpan.TicksPerDay;
        bool hasStartedDay = duration.Ticks % TimeSpan.TicksPerDay > 0;
        return checked((int)fullDays + (hasStartedDay ? 1 : 0));
    }
}
