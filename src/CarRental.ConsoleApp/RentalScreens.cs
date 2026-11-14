using CarRental.Application.Common;
using CarRental.Application.Rentals;
using CarRental.Domain.Rentals;

namespace CarRental.ConsoleApp;

/// <summary>Екрани сценаріїв UC-01 – UC-04: лише ввід і вивід, без правил.</summary>
internal sealed class RentalScreens
{
    private readonly ConsoleIo _io;
    private readonly ReservationService _reservations;
    private readonly HandoverService _handover;
    private readonly TimeProvider _clock;

    /// <summary>Створює екрани.</summary>
    /// <param name="io">Ввід-вивід.</param>
    /// <param name="reservations">Сервіс бронювань.</param>
    /// <param name="handover">Сервіс видачі й повернення.</param>
    /// <param name="clock">Годинник для «зараз».</param>
    public RentalScreens(
        ConsoleIo io, ReservationService reservations, HandoverService handover, TimeProvider clock)
    {
        _io = io;
        _reservations = reservations;
        _handover = handover;
        _clock = clock;
    }

    /// <summary>UC-01: бронювання.</summary>
    public void Reserve()
    {
        int? customer = _io.AskInt("Код клієнта");
        string? plate = customer is null ? null : _io.Ask("Державний номер авто");
        DateTimeOffset? start = plate is null
            ? null
            : _io.AskMoment("Початок оренди", _clock.GetLocalNow());
        int? days = start is null ? null : _io.AskInt("Кількість діб");
        if (days is null)
        {
            return;
        }

        var request = new ReserveRequest(customer!.Value, plate, start!.Value, days.Value);
        Result<Rental> result = _reservations.Reserve(request);
        Show(result, r => $"Заброньовано: договір № {r.Number}, {r.Terms.CarPlate}, "
            + $"{Formats.Moment(r.Terms.Period.Start)} – {Formats.Moment(r.Terms.Period.End)}, "
            + $"{Formats.Money(r.Terms.DailyRate)} за добу");
    }

    /// <summary>UC-02: видача автомобіля.</summary>
    public void PickUp()
    {
        if (_io.AskInt("Номер договору") is int number)
        {
            Show(_handover.PickUp(number), r =>
                $"Видано: договір № {r.Number}, {r.Terms.CarPlate}, "
                + $"о {Formats.Moment(r.PickedUpAt!.Value)}");
        }
    }

    /// <summary>UC-03: закриття оренди з розрахунком.</summary>
    public void Close()
    {
        if (_io.AskInt("Номер договору") is int number)
        {
            Show(_handover.Close(number), c =>
                $"Закрито. Діб: {c.BillableDays}; прокат {Formats.Money(c.RentCost)}; "
                + $"штраф {Formats.Money(c.LateFee)}; до сплати {Formats.Money(c.Total)}");
        }
    }

    /// <summary>UC-04: скасування бронювання.</summary>
    public void Cancel()
    {
        if (_io.AskInt("Номер договору") is int number)
        {
            Show(_reservations.Cancel(number), r => $"Скасовано: договір № {r.Number}");
        }
    }

    private void Show<T>(Result<T> result, Func<T, string> describe) =>
        _io.Line(result.IsSuccess ? describe(result.Value!) : $"Відмова: {result.Error}");
}
