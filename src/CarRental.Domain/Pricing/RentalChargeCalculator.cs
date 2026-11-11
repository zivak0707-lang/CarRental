using CarRental.Domain.Rentals;

namespace CarRental.Domain.Pricing;

/// <summary>
/// Профільне правило варіанта: доба з округленням угору і штраф за пізнє повернення.
/// Таблиця рішень — docs/spec.md, розділ 4.
/// </summary>
public sealed class RentalChargeCalculator
{
    private readonly ILateFeePolicy _lateFee;

    /// <summary>Створює калькулятор із правилом штрафу.</summary>
    /// <param name="lateFee">Правило штрафу за пізнє повернення.</param>
    public RentalChargeCalculator(ILateFeePolicy lateFee)
    {
        ArgumentNullException.ThrowIfNull(lateFee);
        _lateFee = lateFee;
    }

    /// <summary>Рахує доби, вартість прокату і штраф. Побічних ефектів немає.</summary>
    /// <param name="dailyRate">Ціна доби за договором, грн; більша за нуль.</param>
    /// <param name="stay">Фактичне перебування; повернення пізніше за видачу.</param>
    /// <returns>Нарахування за оренду.</returns>
    public RentalCharge Calculate(decimal dailyRate, StayPeriod stay)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(dailyRate);
        bool isLate = stay.Lateness > _lateFee.GracePeriod;
        bool inGrace = stay.Lateness > TimeSpan.Zero && !isLate;

        // Запізнення в межах пільги не подовжує оренду: рахуємо до планового кінця
        DateTimeOffset billedUntil = inGrace ? stay.PlannedReturn : stay.ReturnedAt;
        int days = RentalDuration.StartedDays(billedUntil - stay.PickedUpAt);
        decimal rent = Money.RoundToKopecks(dailyRate * days);
        decimal fee = isLate ? _lateFee.LateFee(dailyRate, stay.Lateness) : 0m;
        return new RentalCharge(days, rent, fee);
    }
}
