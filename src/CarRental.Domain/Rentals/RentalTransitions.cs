using CarRental.Domain.Errors;

namespace CarRental.Domain.Rentals;

/// <summary>Таблиця дозволених переходів стану оренди; решта переходів заборонені.</summary>
public static class RentalTransitions
{
    private static readonly Dictionary<(RentalStatus, RentalEvent), RentalStatus> _allowed = new()
    {
        [(RentalStatus.Reserved, RentalEvent.PickUp)] = RentalStatus.Active,
        [(RentalStatus.Reserved, RentalEvent.Cancel)] = RentalStatus.Cancelled,
        [(RentalStatus.Active, RentalEvent.Close)] = RentalStatus.Closed,
    };

    /// <summary>Повертає новий стан після події.</summary>
    /// <param name="status">Поточний стан.</param>
    /// <param name="rentalEvent">Подія.</param>
    /// <returns>Стан після переходу.</returns>
    /// <exception cref="InvalidRentalStateException">Перехід відсутній у таблиці.</exception>
    public static RentalStatus Next(RentalStatus status, RentalEvent rentalEvent) =>
        _allowed.TryGetValue((status, rentalEvent), out RentalStatus next)
            ? next
            : throw new InvalidRentalStateException(status, rentalEvent);
}
