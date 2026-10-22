using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Core.Services;

/// <summary>Прикладний сервіс бронювання: залежності приходять у конструкторі.</summary>
public sealed class ReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;

    /// <summary>Створює сервіс із переданими залежностями.</summary>
    /// <param name="repository">Сховище бронювань.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    /// <param name="notifier">Сповіщувач клієнта.</param>
    public ReservationService(
        IReservationRepository repository,
        IPricingPolicy pricing,
        INotifier notifier)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
    }

    /// <summary>Приймає бронювання: зберігає його й сповіщає клієнта.</summary>
    /// <param name="reservation">Бронювання.</param>
    public void Place(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        _repository.Add(reservation);
        _notifier.Notify($"бронювання № {reservation.Id} прийнято.");
    }

    /// <summary>Рахує суму бронювання за поточною політикою.</summary>
    /// <param name="reservation">Бронювання.</param>
    /// <returns>Сума до сплати, грн.</returns>
    public decimal TotalOf(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        decimal total = 0m;
        foreach (ReservationDay line in reservation.Lines)
        {
            total += _pricing.PriceOf(line);
        }

        return total;
    }
}
