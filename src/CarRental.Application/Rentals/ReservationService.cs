using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using CarRental.Domain.Cars;
using CarRental.Domain.Errors;
using CarRental.Domain.Rentals;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals;

/// <summary>Сценарії UC-01 «Забронювати автомобіль» і UC-04 «Скасувати бронювання».</summary>
public sealed class ReservationService
{
    /// <summary>Найбільша тривалість оренди, діб.</summary>
    public const int MaxDays = 30;

    /// <summary>На скільки хвилин початок може бути в минулому (час на введення).</summary>
    public const int PastToleranceMinutes = 15;

    private readonly IRentalRepository _rentals;
    private readonly IFleetCatalog _catalog;
    private readonly TimeProvider _clock;
    private readonly ILogger<ReservationService> _log;

    /// <summary>Створює сервіс.</summary>
    /// <param name="rentals">Сховище оренд.</param>
    /// <param name="catalog">Довідники філії.</param>
    /// <param name="clock">Годинник; у тестах — фіксований.</param>
    /// <param name="log">Журнал.</param>
    public ReservationService(
        IRentalRepository rentals,
        IFleetCatalog catalog,
        TimeProvider clock,
        ILogger<ReservationService> log)
    {
        _rentals = rentals ?? throw new ArgumentNullException(nameof(rentals));
        _catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>UC-01: бронює автомобіль. Ввід перевіряється тут, рівно один раз.</summary>
    /// <param name="request">Запит на бронювання.</param>
    /// <returns>Нова оренда у стані Reserved або пояснення відмови.</returns>
    public Result<Rental> Reserve(ReserveRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        _log.LogInformation(
            "UC-01 почато: клієнт {Customer}, авто {Plate}", request.CustomerId, request.Plate);
        string? error = Validate(request, out Car? car);
        if (error is not null)
        {
            _log.LogWarning("UC-01 відхилено: {Reason}", error);
            return Result.Fail<Rental>(error);
        }

        var period = new RentalPeriod(request.Start, request.Start.AddDays(request.Days));
        decimal rate = _catalog.TariffFor(car!.Class).DailyRate;
        var terms = new RentalTerms(request.CustomerId, car.Plate, period, rate);
        var rental = new Rental(_rentals.NextNumber(), terms);
        _rentals.Add(rental);
        _rentals.Save();
        _log.LogInformation("UC-01 завершено: договір {Number}", rental.Number);
        return Result.Ok(rental);
    }

    /// <summary>UC-04: скасовує бронювання до видачі автомобіля.</summary>
    /// <param name="number">Номер договору.</param>
    /// <returns>Скасована оренда або пояснення, якщо її не знайдено.</returns>
    /// <exception cref="InvalidRentalStateException">Оренда вже не в стані Reserved.</exception>
    public Result<Rental> Cancel(int number)
    {
        _log.LogInformation("UC-04 почато: договір {Number}", number);
        Rental? rental = _rentals.Find(number);
        if (rental is null)
        {
            _log.LogWarning("UC-04 відхилено: договору {Number} немає", number);
            return Result.Fail<Rental>($"Договору № {number} не знайдено.");
        }

        try
        {
            rental.Cancel();
        }
        catch (InvalidRentalStateException ex)
        {
            _log.LogWarning("UC-04 відхилено: {Reason}", ex.Message);
            throw;
        }

        _rentals.Save();
        _log.LogInformation("UC-04 завершено: договір {Number} скасовано", number);
        return Result.Ok(rental);
    }

    private static bool IsOpen(Rental rental) =>
        rental.Status is RentalStatus.Reserved or RentalStatus.Active;

    private string? Validate(ReserveRequest request, out Car? car)
    {
        string plate = request.Plate?.Trim().ToUpperInvariant() ?? string.Empty;
        car = plate.Length == 0 ? null : _catalog.FindCar(plate);
        if (plate.Length == 0)
        {
            return "Номер автомобіля не вказано.";
        }

        if (_catalog.FindCustomer(request.CustomerId) is null)
        {
            return $"Клієнта з кодом {request.CustomerId} не знайдено.";
        }

        if (car is null)
        {
            return $"Автомобіля {plate} у парку немає.";
        }

        return ValidatePeriod(request, car);
    }

    private string? ValidatePeriod(ReserveRequest request, Car car)
    {
        if (request.Days < 1 || request.Days > MaxDays)
        {
            return $"Тривалість оренди — від 1 до {MaxDays} діб, а введено {request.Days}.";
        }

        if (request.Start < _clock.GetLocalNow().AddMinutes(-PastToleranceMinutes))
        {
            return "Початок оренди не може бути в минулому.";
        }

        var period = new RentalPeriod(request.Start, request.Start.AddDays(request.Days));
        bool busy = _rentals.All().Any(
            r => r.Terms.CarPlate == car.Plate && IsOpen(r) && r.Terms.Period.Overlaps(period));
        return busy ? $"Автомобіль {car.Plate} уже заброньовано або видано на цей період." : null;
    }
}
