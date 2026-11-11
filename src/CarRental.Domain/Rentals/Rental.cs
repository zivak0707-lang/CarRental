using CarRental.Domain.Pricing;

namespace CarRental.Domain.Rentals;

/// <summary>Документ оренди: від бронювання до закриття або скасування.</summary>
public sealed class Rental
{
    /// <summary>Створює нове бронювання у стані Reserved.</summary>
    /// <param name="number">Номер договору, більший за нуль.</param>
    /// <param name="terms">Умови договору.</param>
    public Rental(int number, RentalTerms terms)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(number);
        ArgumentNullException.ThrowIfNull(terms);
        Number = number;
        Terms = terms;
        Status = RentalStatus.Reserved;
    }

    /// <summary>Номер договору.</summary>
    public int Number { get; }

    /// <summary>Умови договору.</summary>
    public RentalTerms Terms { get; }

    /// <summary>Поточний стан.</summary>
    public RentalStatus Status { get; private set; }

    /// <summary>Фактичний момент видачі; null до видачі.</summary>
    public DateTimeOffset? PickedUpAt { get; private set; }

    /// <summary>Фактичний момент повернення; null до закриття.</summary>
    public DateTimeOffset? ReturnedAt { get; private set; }

    /// <summary>Нарахування; null до закриття.</summary>
    public RentalCharge? Charge { get; private set; }

    /// <summary>Відновлює оренду зі сховища без повторної перевірки переходів.</summary>
    /// <param name="number">Номер договору.</param>
    /// <param name="terms">Умови договору.</param>
    /// <param name="progress">Збережений хід оренди.</param>
    /// <returns>Оренда в збереженому стані.</returns>
    public static Rental Restore(int number, RentalTerms terms, RentalProgress progress)
    {
        ArgumentNullException.ThrowIfNull(progress);
        return new Rental(number, terms)
        {
            Status = progress.Status,
            PickedUpAt = progress.PickedUpAt,
            ReturnedAt = progress.ReturnedAt,
            Charge = progress.Charge,
        };
    }

    /// <summary>Видає автомобіль: Reserved → Active.</summary>
    /// <param name="at">Фактичний момент видачі.</param>
    /// <exception cref="Errors.InvalidRentalStateException">Оренда не в стані Reserved.</exception>
    public void PickUp(DateTimeOffset at)
    {
        Status = RentalTransitions.Next(Status, RentalEvent.PickUp);
        PickedUpAt = at;
    }

    /// <summary>Закриває оренду: Active → Closed і нараховує суму.</summary>
    /// <param name="returnedAt">Фактичний момент повернення; пізніший за видачу.</param>
    /// <param name="calculator">Профільне правило розрахунку.</param>
    /// <returns>Нарахування за оренду.</returns>
    /// <exception cref="Errors.InvalidRentalStateException">Оренда не в стані Active.</exception>
    public RentalCharge Close(DateTimeOffset returnedAt, RentalChargeCalculator calculator)
    {
        ArgumentNullException.ThrowIfNull(calculator);
        RentalStatus next = RentalTransitions.Next(Status, RentalEvent.Close);
        var stay = new StayPeriod(PickedUpAt!.Value, returnedAt, Terms.Period.End);
        Charge = calculator.Calculate(Terms.DailyRate, stay);
        ReturnedAt = returnedAt;
        Status = next;
        return Charge;
    }

    /// <summary>Скасовує бронювання: Reserved → Cancelled.</summary>
    /// <exception cref="Errors.InvalidRentalStateException">Оренда не в стані Reserved.</exception>
    public void Cancel() => Status = RentalTransitions.Next(Status, RentalEvent.Cancel);
}
