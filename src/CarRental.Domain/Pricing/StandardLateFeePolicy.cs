namespace CarRental.Domain.Pricing;

/// <summary>Штраф філії: половина ціни доби за кожну розпочату добу запізнення.</summary>
public sealed class StandardLateFeePolicy : ILateFeePolicy
{
    /// <summary>Частка ціни доби, яку стягують за добу запізнення.</summary>
    public const decimal FeeShare = 0.5m;

    /// <summary>Пільговий період у хвилинах.</summary>
    public const int GraceMinutes = 60;

    /// <inheritdoc/>
    public TimeSpan GracePeriod { get; } = TimeSpan.FromMinutes(GraceMinutes);

    /// <inheritdoc/>
    public decimal LateFee(decimal dailyRate, TimeSpan lateness) =>
        Money.RoundToKopecks(dailyRate * FeeShare * RentalDuration.StartedDays(lateness));
}
