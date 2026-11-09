namespace CarRental.Core.Pricing;

/// <summary>Переведення фактичного часу оренди в доби, що підлягають оплаті.</summary>
public static class RentalDuration
{
    /// <summary>Рахує оплачувані доби: кожна розпочата доба оплачується повністю.</summary>
    /// <param name="pickedUpAt">Момент видачі автомобіля.</param>
    /// <param name="returnedAt">Момент повернення автомобіля, пізніший за видачу.</param>
    /// <returns>Кількість діб, округлена вгору.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Повернення не пізніше за видачу.</exception>
    public static int BillableDays(DateTimeOffset pickedUpAt, DateTimeOffset returnedAt)
    {
        if (returnedAt <= pickedUpAt)
        {
            throw new ArgumentOutOfRangeException(
                nameof(returnedAt),
                returnedAt,
                "Момент повернення має бути пізнішим за момент видачі.");
        }

        // Рахуємо в тіках, а не через TotalDays: double дає похибку саме на межі доби
        long ticks = (returnedAt - pickedUpAt).Ticks;
        long fullDays = ticks / TimeSpan.TicksPerDay;
        bool hasStartedDay = ticks % TimeSpan.TicksPerDay > 0;
        return checked((int)fullDays + (hasStartedDay ? 1 : 0));
    }
}
