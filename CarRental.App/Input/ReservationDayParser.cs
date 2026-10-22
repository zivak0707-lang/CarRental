using System.Globalization;
using CarRental.Core.Domain;
using CarRental.Core.Errors;

namespace CarRental.App.Input;

/// <summary>
/// Барикада: перетворює рядки, введені користувачем, на перевірену позицію бронювання.
/// Ядро отримує лише готовий ReservationDay і про формат консолі нічого не знає.
/// </summary>
internal static class ReservationDayParser
{
    private const string DateFormat = "dd.MM.yyyy";

    /// <summary>Українська культура: дати дд.мм.рррр, дробова частина через кому.</summary>
    private static CultureInfo Ukrainian => CultureInfo.GetCultureInfo("uk-UA");

    /// <summary>Розбирає чотири рядки вводу на позицію бронювання.</summary>
    /// <param name="plate">Державний номер.</param>
    /// <param name="startDate">Дата видачі, дд.мм.рррр.</param>
    /// <param name="endDate">Дата повернення, дд.мм.рррр.</param>
    /// <param name="dailyRate">Ставка за добу, наприклад 250,50.</param>
    /// <returns>Готова позиція або пояснення помилки вводу.</returns>
    public static Result<ReservationDay> Parse(
        string? plate,
        string? startDate,
        string? endDate,
        string? dailyRate)
    {
        if (string.IsNullOrWhiteSpace(plate))
        {
            return Result.Fail<ReservationDay>("Державний номер не може бути порожнім.");
        }

        if (!DateOnly.TryParseExact(
            startDate, DateFormat, Ukrainian, DateTimeStyles.None, out DateOnly start))
        {
            return Result.Fail<ReservationDay>(
                "Дата видачі — у форматі дд.мм.рррр, наприклад 02.11.2026.");
        }

        if (!DateOnly.TryParseExact(
            endDate, DateFormat, Ukrainian, DateTimeStyles.None, out DateOnly end))
        {
            return Result.Fail<ReservationDay>(
                "Дата повернення — у форматі дд.мм.рррр, наприклад 04.11.2026.");
        }

        if (end <= start)
        {
            return Result.Fail<ReservationDay>(
                "Дата повернення має бути пізнішою за дату видачі.");
        }

        if (!decimal.TryParse(dailyRate, NumberStyles.Number, Ukrainian, out decimal rate)
            || rate <= 0m)
        {
            return Result.Fail<ReservationDay>(
                "Ставка — число більше 0, дробова частина через кому, наприклад 250,50.");
        }

        return Result.Ok(new ReservationDay(plate.Trim().ToUpperInvariant(), start, end, rate));
    }
}
