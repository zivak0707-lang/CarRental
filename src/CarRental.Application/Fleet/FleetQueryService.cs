using CarRental.Application.Abstractions;
using CarRental.Domain.Cars;
using CarRental.Domain.Rentals;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Fleet;

/// <summary>Сценарії UC-05 «Знайти оренди» і UC-06 «Звіт про зайнятість автопарку».</summary>
public sealed class FleetQueryService
{
    private readonly IRentalRepository _rentals;
    private readonly IFleetCatalog _catalog;
    private readonly ILogger<FleetQueryService> _log;

    /// <summary>Створює сервіс.</summary>
    /// <param name="rentals">Сховище оренд.</param>
    /// <param name="catalog">Довідники філії.</param>
    /// <param name="log">Журнал.</param>
    public FleetQueryService(
        IRentalRepository rentals, IFleetCatalog catalog, ILogger<FleetQueryService> log)
    {
        _rentals = rentals ?? throw new ArgumentNullException(nameof(rentals));
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>UC-05: шукає оренди за станом, клієнтом і номером авто одночасно.</summary>
    /// <param name="query">Критерії пошуку.</param>
    /// <returns>Знайдені оренди в порядку номерів договорів.</returns>
    public IReadOnlyList<Rental> Find(RentalQuery query)
    {
        ArgumentNullException.ThrowIfNull(query);
        List<Rental> found = _rentals.All()
            .Where(r => query.Status is null || r.Status == query.Status)
            .Where(r => Matches(r.Terms.CarPlate, query.Plate))
            .Where(r => Matches(CustomerName(r), query.CustomerName))
            .OrderBy(r => r.Number)
            .ToList();
        _log.LogInformation("UC-05 завершено: знайдено {Count}", found.Count);
        return found;
    }

    /// <summary>UC-06: рахує видані, заброньовані й вільні авто кожного класу на дату.</summary>
    /// <param name="day">Дата звіту.</param>
    /// <returns>Звіт із рядком на кожен клас і підсумком.</returns>
    public OccupancyReport Occupancy(DateOnly day)
    {
        IReadOnlyList<Rental> rentals = _rentals.All();
        HashSet<string> rented = Plates(rentals, r => r.Status == RentalStatus.Active);
        HashSet<string> reserved = Plates(
            rentals, r => r.Status == RentalStatus.Reserved && r.Terms.Period.Covers(day));
        reserved.ExceptWith(rented);
        List<OccupancyRow> rows = Enum.GetValues<CarClass>()
            .Select(c => RowFor(c, rented, reserved))
            .ToList();
        var total = new OccupancyRow(
            null, rows.Sum(r => r.Total), rows.Sum(r => r.Rented), rows.Sum(r => r.Reserved));
        _log.LogInformation("UC-06 завершено: {Day}, зайнятість {Percent} %", day, total.Percent);
        return new OccupancyReport(day, rows, total);
    }

    private static bool Matches(string? value, string? fragment) =>
        string.IsNullOrWhiteSpace(fragment)
        || (value?.Contains(fragment.Trim(), StringComparison.OrdinalIgnoreCase) ?? false);

    private static HashSet<string> Plates(IEnumerable<Rental> rentals, Func<Rental, bool> filter) =>
        rentals.Where(filter).Select(r => r.Terms.CarPlate).ToHashSet(StringComparer.Ordinal);

    private string? CustomerName(Rental rental) =>
        _catalog.FindCustomer(rental.Terms.CustomerId)?.FullName;

    private OccupancyRow RowFor(CarClass carClass, HashSet<string> rented, HashSet<string> reserved)
    {
        List<string> plates = _catalog.Cars
            .Where(c => c.Class == carClass)
            .Select(c => c.Plate)
            .ToList();
        return new OccupancyRow(
            carClass, plates.Count, plates.Count(rented.Contains), plates.Count(reserved.Contains));
    }
}
