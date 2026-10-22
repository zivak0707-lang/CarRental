namespace CarRental.Core.Domain;

/// <summary>
/// Позиція бронювання: один автомобіль на період дат за фіксованою ставкою.
/// Незмінна: після створення її не можна зіпсувати.
/// </summary>
public sealed record ReservationDay
{
    /// <summary>Створює позицію й перевіряє її інваріант.</summary>
    /// <param name="plate">Державний номер автомобіля, не порожній.</param>
    /// <param name="startDate">Дата видачі автомобіля.</param>
    /// <param name="endDate">Дата повернення, пізніша за дату видачі.</param>
    /// <param name="dailyRate">Ставка за добу, грн, більша за нуль.</param>
    public ReservationDay(string plate, DateOnly startDate, DateOnly endDate, decimal dailyRate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(plate);
        if (endDate <= startDate)
        {
            throw new ArgumentOutOfRangeException(
                nameof(endDate),
                endDate,
                "Дата повернення має бути пізнішою за дату видачі.");
        }

        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dailyRate);

        Plate = plate;
        StartDate = startDate;
        EndDate = endDate;
        DailyRate = dailyRate;
    }

    /// <summary>Державний номер автомобіля.</summary>
    public string Plate { get; }

    /// <summary>Дата видачі автомобіля.</summary>
    public DateOnly StartDate { get; }

    /// <summary>Дата повернення автомобіля.</summary>
    public DateOnly EndDate { get; }

    /// <summary>Ставка за добу, грн.</summary>
    public decimal DailyRate { get; }

    /// <summary>Тривалість оренди в добах; обчислюється, а не зберігається.</summary>
    public int Days => EndDate.DayNumber - StartDate.DayNumber;

    /// <summary>Вартість позиції без знижок: доби, помножені на ставку.</summary>
    public decimal Amount => Days * DailyRate;
}
