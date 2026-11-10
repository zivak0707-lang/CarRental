using CarRental.Domain.Rentals;

namespace CarRental.Domain.Errors;

/// <summary>Подію не можна застосувати до оренди в її поточному стані.</summary>
public sealed class InvalidRentalStateException : DomainRuleException
{
    /// <summary>Створює виняток для забороненого переходу.</summary>
    /// <param name="status">Поточний стан оренди.</param>
    /// <param name="rentalEvent">Подія, яку відхилено.</param>
    public InvalidRentalStateException(RentalStatus status, RentalEvent rentalEvent)
        : base(
            "rental.transition",
            $"Дію «{NameOf(rentalEvent)}» заборонено для оренди у стані {status}.")
    {
        Status = status;
        Event = rentalEvent;
    }

    /// <summary>Стан оренди в момент спроби.</summary>
    public RentalStatus Status { get; }

    /// <summary>Подія, яку відхилено.</summary>
    public RentalEvent Event { get; }

    private static string NameOf(RentalEvent rentalEvent) => rentalEvent switch
    {
        RentalEvent.PickUp => "видати",
        RentalEvent.Close => "закрити",
        _ => "скасувати",
    };
}
