namespace CarRental.Domain.Pricing;

/// <summary>Фактичне перебування автомобіля в клієнта і запланований кінець оренди.</summary>
/// <param name="PickedUpAt">Фактичний момент видачі.</param>
/// <param name="ReturnedAt">Фактичний момент повернення.</param>
/// <param name="PlannedReturn">Запланований момент повернення за договором.</param>
public readonly record struct StayPeriod(
    DateTimeOffset PickedUpAt, DateTimeOffset ReturnedAt, DateTimeOffset PlannedReturn)
{
    /// <summary>Запізнення: від'ємне або нульове, якщо авто повернули вчасно.</summary>
    public TimeSpan Lateness => ReturnedAt - PlannedReturn;
}
