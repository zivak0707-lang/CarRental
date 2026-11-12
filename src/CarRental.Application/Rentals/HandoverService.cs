using CarRental.Application.Abstractions;
using CarRental.Application.Common;
using CarRental.Domain.Errors;
using CarRental.Domain.Pricing;
using CarRental.Domain.Rentals;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Rentals;

/// <summary>Сценарії-ядра варіанта: UC-02 «Видати автомобіль» і UC-03 «Закрити оренду».</summary>
public sealed class HandoverService
{
    private readonly IRentalRepository _rentals;
    private readonly RentalChargeCalculator _calculator;
    private readonly TimeProvider _clock;
    private readonly ILogger<HandoverService> _log;

    /// <summary>Створює сервіс.</summary>
    /// <param name="rentals">Сховище оренд.</param>
    /// <param name="calculator">Профільне правило розрахунку.</param>
    /// <param name="clock">Годинник; у тестах — фіксований.</param>
    /// <param name="log">Журнал.</param>
    public HandoverService(
        IRentalRepository rentals,
        RentalChargeCalculator calculator,
        TimeProvider clock,
        ILogger<HandoverService> log)
    {
        _rentals = rentals ?? throw new ArgumentNullException(nameof(rentals));
        _calculator = calculator ?? throw new ArgumentNullException(nameof(calculator));
        _clock = clock ?? throw new ArgumentNullException(nameof(clock));
        _log = log ?? throw new ArgumentNullException(nameof(log));
    }

    /// <summary>UC-02: видає заброньований автомобіль клієнту зараз.</summary>
    /// <param name="number">Номер договору.</param>
    /// <returns>Оренда у стані Active або пояснення відмови.</returns>
    /// <exception cref="InvalidRentalStateException">Оренда не в стані Reserved.</exception>
    public Result<Rental> PickUp(int number)
    {
        _log.LogInformation("UC-02 почато: договір {Number}", number);
        Rental? rental = _rentals.Find(number);
        DateTimeOffset now = _clock.GetLocalNow();
        string? error = rental is null ? $"Договору № {number} не знайдено."
            : now >= rental.Terms.Period.End ? "Період оренди минув; потрібне нове бронювання."
            : null;
        if (error is not null)
        {
            _log.LogWarning("UC-02 відхилено: {Reason}", error);
            return Result.Fail<Rental>(error);
        }

        Apply(rental!, r => r.PickUp(now), "UC-02");
        _log.LogInformation("UC-02 завершено: {Number}, {Plate}", number, rental!.Terms.CarPlate);
        return Result.Ok(rental);
    }

    /// <summary>UC-03: приймає автомобіль зараз і нараховує суму за профільним правилом.</summary>
    /// <param name="number">Номер договору.</param>
    /// <returns>Нарахування або пояснення відмови.</returns>
    /// <exception cref="InvalidRentalStateException">Оренда не в стані Active.</exception>
    public Result<RentalCharge> Close(int number)
    {
        _log.LogInformation("UC-03 почато: договір {Number}", number);
        Rental? rental = _rentals.Find(number);
        if (rental is null)
        {
            _log.LogWarning("UC-03 відхилено: договору {Number} немає", number);
            return Result.Fail<RentalCharge>($"Договору № {number} не знайдено.");
        }

        Apply(rental, r => r.Close(_clock.GetLocalNow(), _calculator), "UC-03");
        _log.LogInformation("UC-03 завершено: {Number}, {Total} грн", number, rental.Charge!.Total);
        return Result.Ok(rental.Charge);
    }

    private void Apply(Rental rental, Action<Rental> change, string scenario)
    {
        try
        {
            change(rental);
        }
        catch (InvalidRentalStateException ex)
        {
            _log.LogWarning("{Scenario} відхилено: {Reason}", scenario, ex.Message);
            throw;
        }

        _rentals.Save();
    }
}
