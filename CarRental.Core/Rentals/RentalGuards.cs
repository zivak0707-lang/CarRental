using CarRental.Core.Domain;

namespace CarRental.Core.Rentals;

/// <summary>Перевірки передумов для розрахунку підсумку прокату.</summary>
internal static class RentalGuards
{
    /// <summary>Перевіряє заголовок прокату: номер, ім'я та пошту клієнта.</summary>
    /// <param name="rentalId">Номер прокату, має бути додатним.</param>
    /// <param name="customerName">Ім'я клієнта, не порожнє.</param>
    /// <param name="customerEmail">Пошта клієнта, має містити символ @.</param>
    public static void EnsureHeaderValid(
        int rentalId,
        string customerName,
        string customerEmail)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(rentalId);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerName);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerEmail);
        if (!customerEmail.Contains('@'))
        {
            throw new ArgumentException(
                "Пошта має містити символ @.",
                nameof(customerEmail));
        }
    }

    /// <summary>Перевіряє один рядок прокату: номер авто, кількість діб і ставку.</summary>
    /// <param name="line">Рядок прокату.</param>
    public static void EnsureLineValid(RentalLine line)
    {
        ArgumentNullException.ThrowIfNull(line);
        ArgumentException.ThrowIfNullOrWhiteSpace(line.Plate);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(line.Days, nameof(line.Days));
        ArgumentOutOfRangeException.ThrowIfNegative(line.DailyRate, nameof(line.DailyRate));
    }

    /// <summary>Перевіряє рядки прокату: колекція не порожня, кожен рядок коректний.</summary>
    /// <param name="lines">Рядки прокату.</param>
    public static void EnsureLinesValid(IReadOnlyList<RentalLine> lines)
    {
        ArgumentNullException.ThrowIfNull(lines);
        if (lines.Count == 0)
        {
            throw new ArgumentException(
                "Прокат не містить жодного рядка.",
                nameof(lines));
        }

        foreach (RentalLine line in lines)
        {
            EnsureLineValid(line);
        }
    }

    /// <summary>Перевіряє грошову суму: вона не може бути від'ємною.</summary>
    /// <param name="amount">Сума, грн.</param>
    public static void EnsureAmountValid(decimal amount)
    {
        if (amount < 0m)
        {
            throw new ArgumentOutOfRangeException(
                nameof(amount),
                amount,
                "Сума не може бути від'ємною.");
        }
    }

    /// <summary>Перевіряє запит на розрахунок цілком: заголовок, рядки й вартість подачі.</summary>
    /// <param name="request">Запит на розрахунок.</param>
    public static void EnsureRequestValid(TotalRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(request.Customer);
        EnsureHeaderValid(
            request.RentalId,
            request.Customer.Name,
            request.Customer.Email);
        EnsureLinesValid(request.Lines);
        ArgumentOutOfRangeException.ThrowIfNegative(request.DeliveryPrice);
    }
}
