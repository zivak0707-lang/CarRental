using CarRental.Core.Abstractions;
using CarRental.Core.Domain;
using CarRental.Core.Errors;

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

    /// <summary>Додає перевірену позицію до чернетки бронювання.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <param name="line">Позиція, уже розібрана й перевірена на барикаді.</param>
    public void AddLine(int reservationId, ReservationDay line)
    {
        ArgumentNullException.ThrowIfNull(line);
        Reservation reservation = _repository.GetById(reservationId)
            ?? throw new DomainRuleException(
                "reservation.exists",
                $"Бронювання № {reservationId} не знайдено.");
        if (reservation.Status != ReservationStatus.Draft)
        {
            throw new DomainRuleException(
                "reservation.editable",
                $"Бронювання № {reservationId} у стані {reservation.Status} не редагується.");
        }

        reservation.AddLine(line);
    }

    /// <summary>ЛР5, стан before: коди повернення без назв і сенсу.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <param name="amount">Сплачена сума, грн.</param>
    /// <returns>0 — успіх; -1, -2, -3 — помилки.</returns>
    public int Pay(int reservationId, decimal amount)
    {
        var reservation = _repository.GetById(reservationId);
        if (reservation == null)
        {
            return -1;
        }

        if (reservation.Status != ReservationStatus.Confirmed)
        {
            return -2;
        }

        if (amount < TotalOf(reservation))
        {
            return -3;
        }

        reservation.PickUp();
        return 0;
    }

    /// <summary>ЛР5, стан before: проковтнутий виняток і повернення null.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <returns>Бронювання або null.</returns>
    public Reservation? Load(int reservationId)
    {
        try
        {
            return _repository.GetById(reservationId);
        }
        catch (Exception)
        {
        }

        return null;
    }

    /// <summary>ЛР5, стан before: широке перехоплення, консоль у ядрі й throw ex.</summary>
    /// <param name="reservationId">Номер бронювання.</param>
    /// <param name="reason">Причина скасування.</param>
    public void Cancel(int reservationId, string reason)
    {
        try
        {
            var reservation = _repository.GetById(reservationId);
            reservation!.Cancel();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Щось пішло не так");
#pragma warning disable CA2200 // стан before для ЛР5; виправляється в завданні 5
            throw ex;
#pragma warning restore CA2200
        }
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
