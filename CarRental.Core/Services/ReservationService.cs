using CarRental.Core.Abstractions;
using CarRental.Core.Domain;

namespace CarRental.Core.Services;

/// <summary>Прикладний сервіс бронювання: залежності приходять у конструкторі.</summary>
public sealed class ReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IPricingPolicy _pricing;

    /// <summary>Створює сервіс із переданими залежностями.</summary>
    /// <param name="repository">Сховище бронювань.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    public ReservationService(IReservationRepository repository, IPricingPolicy pricing)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
    }

    /// <summary>Приймає бронювання: зберігає його у сховищі.</summary>
    /// <param name="reservation">Бронювання.</param>
    public void Place(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        _repository.Add(reservation);
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
