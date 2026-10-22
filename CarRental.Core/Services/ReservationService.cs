using System.Diagnostics;
using CarRental.Core.Abstractions;
using CarRental.Core.Domain;
using CarRental.Core.Errors;
using Microsoft.Extensions.Logging;

namespace CarRental.Core.Services;

/// <summary>Прикладний сервіс бронювання: залежності приходять у конструкторі.</summary>
public sealed class ReservationService
{
    private readonly IReservationRepository _repository;
    private readonly IPricingPolicy _pricing;
    private readonly INotifier _notifier;
    private readonly ILogger<ReservationService> _log;

    /// <summary>Створює сервіс із переданими залежностями.</summary>
    /// <param name="repository">Сховище бронювань.</param>
    /// <param name="pricing">Політика розрахунку ціни.</param>
    /// <param name="notifier">Сповіщувач клієнта.</param>
    /// <param name="log">Журнал сервісу.</param>
    public ReservationService(
        IReservationRepository repository,
        IPricingPolicy pricing,
        INotifier notifier,
        ILogger<ReservationService> log)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _pricing = pricing ?? throw new ArgumentNullException(nameof(pricing));
        _notifier = notifier ?? throw new ArgumentNullException(nameof(notifier));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>Приймає бронювання: зберігає його й сповіщає клієнта.</summary>
    /// <param name="reservation">Бронювання.</param>
    public void Place(Reservation reservation)
    {
        ArgumentNullException.ThrowIfNull(reservation);
        _repository.Add(reservation);
        _notifier.Notify($"бронювання № {reservation.Id} прийнято.");
        _log.LogInformation("Бронювання {ReservationId} прийнято", reservation.Id);
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

        Debug.Assert(total >= 0m, "Сума бронювання не може бути від'ємною.");
        return total;
    }

    /// <summary>Додає перевірену позицію до чернетки бронювання.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <param name="line">Позиція, уже розібрана й перевірена на барикаді.</param>
    public void AddLine(int reservationId, ReservationDay line)
    {
        ArgumentNullException.ThrowIfNull(line);
        using IDisposable? scope = _log.BeginScope("Reservation:{ReservationId}", reservationId);
        _log.LogDebug("Додавання позиції {Plate} на {Days} доб", line.Plate, line.Days);
        Reservation reservation = Load(reservationId);
        if (reservation.Status != ReservationStatus.Draft)
        {
            throw new DomainRuleException(
                "reservation.editable",
                $"Бронювання № {reservationId} у стані {reservation.Status} не редагується.");
        }

        reservation.AddLine(line);
    }

    /// <summary>Оплата: після повної оплати автомобіль видається (Confirmed → Active).</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <param name="amount">Сплачена сума, грн.</param>
    public void Pay(int reservationId, decimal amount)
    {
        using IDisposable? scope = _log.BeginScope("Reservation:{ReservationId}", reservationId);
        _log.LogDebug("Початок оплати, сума {Amount}", amount);
        Reservation reservation = Load(reservationId);
        if (reservation.Status != ReservationStatus.Confirmed)
        {
            throw new DomainRuleException(
                "reservation.payable",
                $"Стан {reservation.Status} не дозволяє оплату бронювання № {reservationId}.");
        }

        decimal total = TotalOf(reservation);
        if (amount < total)
        {
            throw new DomainRuleException(
                "reservation.amount",
                $"Сплачено {amount:0.00} грн, а треба {total:0.00} грн.");
        }

        if (amount > total)
        {
            _log.LogWarning("Переплата {Extra} грн", amount - total);
        }

        reservation.PickUp();
        _log.LogInformation("Оплачено {Amount} грн, автомобіль видано", amount);
    }

    /// <summary>Знаходить бронювання або кидає DomainRuleException, якщо його немає.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <returns>Знайдене бронювання (ніколи не null).</returns>
    public Reservation Load(int reservationId) =>
        _repository.GetById(reservationId)
        ?? throw new DomainRuleException(
            "reservation.exists",
            $"Бронювання № {reservationId} не знайдено.");

    /// <summary>Скасовує бронювання; дозволено лише до видачі автомобіля.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <param name="reason">Причина скасування, не порожня.</param>
    public void Cancel(int reservationId, string reason)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);
        using IDisposable? scope = _log.BeginScope("Reservation:{ReservationId}", reservationId);
        Reservation reservation = Load(reservationId);
        try
        {
            reservation.Cancel();
        }
        catch (InvalidOperationException ex)
        {
            _log.LogError(ex, "Скасування неможливе у стані {Status}", reservation.Status);
            throw new DomainRuleException(
                "reservation.cancellable",
                $"Бронювання № {reservationId} у стані {reservation.Status} скасувати не можна.",
                ex);
        }

        _log.LogInformation("Бронювання скасовано, причина: {Reason}", reason);
    }

    /// <summary>ЛР5, стан before: при винятку файл лишається відкритим.</summary>
    /// <param name="path">Шлях до файлу звіту.</param>
    /// <param name="reservation">Бронювання.</param>
    public void ExportReport(string path, Reservation reservation)
    {
        var writer = new StreamWriter(path);
        writer.WriteLine($"Бронювання {reservation.Id}");
        writer.WriteLine(TotalOf(reservation));
        writer.Close();
    }
}
