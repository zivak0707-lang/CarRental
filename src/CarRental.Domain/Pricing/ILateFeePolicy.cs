namespace CarRental.Domain.Pricing;

/// <summary>Правило штрафу за пізнє повернення (точка розширення).</summary>
public interface ILateFeePolicy
{
    /// <summary>Запізнення, яке не подовжує оренду і не штрафується.</summary>
    TimeSpan GracePeriod { get; }

    /// <summary>Штраф за запізнення понад пільговий період.</summary>
    /// <param name="dailyRate">Ціна доби за договором, грн.</param>
    /// <param name="lateness">Запізнення, більше за пільговий період.</param>
    /// <returns>Штраф, грн, округлений до копійок.</returns>
    decimal LateFee(decimal dailyRate, TimeSpan lateness);
}
