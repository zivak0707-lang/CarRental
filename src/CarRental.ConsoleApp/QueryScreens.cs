using System.Globalization;
using CarRental.Application.Abstractions;
using CarRental.Application.Fleet;
using CarRental.Domain.Rentals;

namespace CarRental.ConsoleApp;

/// <summary>Екрани UC-05 «Знайти оренди» і UC-06 «Зайнятість автопарку».</summary>
internal sealed class QueryScreens
{
    private readonly ConsoleIo _io;
    private readonly FleetQueryService _fleet;
    private readonly IFleetCatalog _catalog;
    private readonly TimeProvider _clock;

    /// <summary>Створює екрани.</summary>
    /// <param name="io">Ввід-вивід.</param>
    /// <param name="fleet">Сервіс пошуку і звітів.</param>
    /// <param name="catalog">Довідники для імен клієнтів.</param>
    /// <param name="clock">Годинник для «сьогодні».</param>
    public QueryScreens(
        ConsoleIo io, FleetQueryService fleet, IFleetCatalog catalog, TimeProvider clock)
    {
        _io = io;
        _fleet = fleet;
        _catalog = catalog;
        _clock = clock;
    }

    /// <summary>UC-05: пошук оренд за станом, клієнтом і номером авто.</summary>
    public void Find()
    {
        _io.Line("Стан: 1 — заброньовано, 2 — видано, 3 — закрито, 4 — скасовано");
        string? status = _io.Ask("Стан (Enter — будь-який)");
        string? name = status is null ? null : _io.Ask("Частина імені клієнта (Enter — будь-яке)");
        string? plate = name is null ? null : _io.Ask("Державний номер (Enter — будь-який)");
        if (plate is null)
        {
            return;
        }

        var query = new RentalQuery(ParseStatus(status), name, plate);
        IReadOnlyList<Rental> found = _fleet.Find(query);
        _io.Line(found.Count == 0 ? "Нічого не знайдено." : $"Знайдено: {found.Count}");
        foreach (Rental r in found)
        {
            string customer = _catalog.FindCustomer(r.Terms.CustomerId)?.FullName ?? "?";
            _io.Line($"№ {r.Number} | {r.Terms.CarPlate,-8} | {customer,-24} | "
                + $"{Formats.Status(r.Status),-12} | {Formats.Moment(r.Terms.Period.Start)} – "
                + Formats.Moment(r.Terms.Period.End));
        }
    }

    /// <summary>UC-06: звіт «Зайнятість автопарку» на дату.</summary>
    public void Occupancy()
    {
        DateOnly today = DateOnly.FromDateTime(_clock.GetLocalNow().DateTime);
        if (_io.AskDate("Дата звіту", today) is not DateOnly day)
        {
            return;
        }

        OccupancyReport report = _fleet.Occupancy(day);
        _io.Line($"Зайнятість автопарку на {day:dd.MM.yyyy}");
        _io.Line("Клас      | Усього | Видано | Заброньовано | Вільно | Зайнятість");
        foreach (OccupancyRow row in report.Rows.Append(report.Total))
        {
            string name = row.Class?.ToString() ?? "Разом";
            _io.Line($"{name,-9} | {row.Total,6} | {row.Rented,6} | {row.Reserved,12} | "
                + $"{row.Free,6} | "
                + $"{Formats.Number(row.Percent),8} %");
        }
    }

    // Номер пункту 1–4 відповідає стану Reserved–Cancelled; інше означає «будь-який»
    private static RentalStatus? ParseStatus(string? text) =>
        int.TryParse(text, CultureInfo.InvariantCulture, out int code)
        && Enum.IsDefined(typeof(RentalStatus), code - 1)
            ? (RentalStatus)(code - 1)
            : null;
}
