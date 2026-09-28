namespace CarRental.Core.Domain;

/// <summary>Прокат автомобілів для клієнта.</summary>
public sealed class Rental
{
    private readonly List<RentalLine> _lines = new();

    public int Id { get; init; }

    public int CustomerId { get; init; }

    public RentalStatus Status { get; set; } = RentalStatus.New;

    public DateTimeOffset CreatedAt { get; init; }

    public IReadOnlyList<RentalLine> Lines => _lines;

    /// <summary>Додає рядок до прокату.</summary>
    public void AddLine(RentalLine line) => _lines.Add(line);

    /// <summary>Обчислює підсумкову вартість прокату.</summary>
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