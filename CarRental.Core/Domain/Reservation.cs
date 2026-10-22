namespace CarRental.Core.Domain;

/// <summary>
/// Бронювання — агрегат: позиції приховано, стан змінюють лише методи-переходи.
/// </summary>
public sealed class Reservation
{
    private readonly List<ReservationDay> _lines = new();

    /// <summary>Створює бронювання у стані чернетки.</summary>
    /// <param name="id">Номер бронювання, більший за нуль.</param>
    /// <param name="renterId">Ідентифікатор орендаря, більший за нуль.</param>
    /// <param name="createdAt">Дата створення.</param>
    public Reservation(int id, int renterId, DateOnly createdAt)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(renterId);

        Id = id;
        RenterId = renterId;
        CreatedAt = createdAt;
        Status = ReservationStatus.Draft;
    }

    /// <summary>Номер бронювання.</summary>
    public int Id { get; }

    /// <summary>Ідентифікатор орендаря.</summary>
    public int RenterId { get; }

    /// <summary>Дата створення.</summary>
    public DateOnly CreatedAt { get; }

    /// <summary>Поточний стан; змінюється лише методами-переходами.</summary>
    public ReservationStatus Status { get; private set; }

    /// <summary>Позиції лише для читання: додати їх можна тільки через AddLine.</summary>
    public IReadOnlyList<ReservationDay> Lines => _lines.AsReadOnly();

    /// <summary>Додає позицію; дозволено лише в чернетці.</summary>
    /// <param name="line">Позиція бронювання.</param>
    public void AddLine(ReservationDay line)
    {
        ArgumentNullException.ThrowIfNull(line);
        if (Status != ReservationStatus.Draft)
        {
            throw new InvalidOperationException("Позиції додають лише до чернетки бронювання.");
        }

        _lines.Add(line);
    }

    /// <summary>Перехід Draft → Confirmed; потрібна хоча б одна позиція.</summary>
    public void Confirm()
    {
        EnsureStatus(ReservationStatus.Draft);
        if (_lines.Count == 0)
        {
            throw new InvalidOperationException("Бронювання без позицій підтвердити не можна.");
        }

        Status = ReservationStatus.Confirmed;
    }

    /// <summary>Перехід Confirmed → Active: автомобіль видано клієнтові.</summary>
    public void PickUp()
    {
        EnsureStatus(ReservationStatus.Confirmed);
        Status = ReservationStatus.Active;
    }

    /// <summary>Перехід Active → Closed: автомобіль повернуто.</summary>
    public void Close()
    {
        EnsureStatus(ReservationStatus.Active);
        Status = ReservationStatus.Closed;
    }

    /// <summary>Скасування; дозволене лише до видачі автомобіля (з Draft або Confirmed).</summary>
    public void Cancel()
    {
        if (Status != ReservationStatus.Draft && Status != ReservationStatus.Confirmed)
        {
            throw new InvalidOperationException($"Скасувати бронювання у стані {Status} не можна.");
        }

        Status = ReservationStatus.Cancelled;
    }

    private void EnsureStatus(ReservationStatus expected)
    {
        if (Status != expected)
        {
            throw new InvalidOperationException(
                $"Перехід заборонено: очікувався стан {expected}, а поточний — {Status}.");
        }
    }
}
