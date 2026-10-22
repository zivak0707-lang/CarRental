namespace CarRental.Core.Domain;

/// <summary>
/// Архівне бронювання НЕ є бронюванням: воно містить його всередині
/// й показує лише дозволені операції — читання.
/// </summary>
public sealed class ArchivedReservation
{
    private readonly Reservation _reservation;

    /// <summary>Архівує закрите або скасоване бронювання.</summary>
    /// <param name="reservation">Бронювання у стані Closed або Cancelled.</param>
    /// <param name="archivedAt">Дата архівування.</param>
    public ArchivedReservation(Reservation reservation, DateOnly archivedAt)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        if (reservation.Status != ReservationStatus.Closed
            && reservation.Status != ReservationStatus.Cancelled)
        {
            throw new InvalidOperationException(
                "Архівувати можна лише закрите або скасоване бронювання.");
        }

        _reservation = reservation;
        ArchivedAt = archivedAt;
    }

    /// <summary>Дата архівування.</summary>
    public DateOnly ArchivedAt { get; }

    /// <summary>Номер бронювання.</summary>
    public int Id => _reservation.Id;

    /// <summary>Стан, у якому бронювання потрапило до архіву.</summary>
    public ReservationStatus Status => _reservation.Status;

    /// <summary>Позиції бронювання лише для читання.</summary>
    public IReadOnlyList<ReservationDay> Lines => _reservation.Lines;

    // Методу AddLine немає взагалі: порушити контракт нічим
}
