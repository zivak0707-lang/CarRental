namespace CarRental.Core.Domain;

/// <summary>Прокат автомобілів для клієнта.</summary>
public sealed class Rental
{
    private readonly List<RentalLine> _lines = new();

    /// <summary>Ідентифікатор прокату.</summary>
    public int Id { get; init; }

    /// <summary>Ідентифікатор клієнта, якому належить прокат.</summary>
    public int CustomerId { get; init; }

    /// <summary>Поточний стан прокату.</summary>
    public RentalStatus Status { get; set; } = RentalStatus.New;

    /// <summary>Дата й час створення прокату.</summary>
    public DateTimeOffset CreatedAt { get; init; }

    /// <summary>Рядки прокату (лише для читання).</summary>
    public IReadOnlyList<RentalLine> Lines => _lines;

    /// <summary>Додає рядок до прокату.</summary>
    /// <param name="line">Рядок прокату, який додається.</param>
    public void AddLine(RentalLine line) => _lines.Add(line);

    /// <summary>Обчислює підсумкову вартість прокату.</summary>
    /// <returns>Сума вартостей усіх рядків прокату.</returns>
    public decimal Total()
    {
        decimal sum = 0m;

        foreach (RentalLine line in _lines)
        {
            sum += line.Amount;
        }

        return sum;
    }
}
