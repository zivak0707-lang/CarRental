using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Core.Pricing;

/// <summary>Ціна для VIP-клієнта: знижка 10 % на кожну позицію.</summary>
public sealed class VipPricingPolicy : IPricingPolicy
{
    private const decimal VipRate = 0.10m;

    /// <inheritdoc/>
    public decimal PriceOf(ReservationDay line)
    {
        ArgumentNullException.ThrowIfNull(line);
        return Math.Round(line.Amount * (1m - VipRate), 2);
    }
}
