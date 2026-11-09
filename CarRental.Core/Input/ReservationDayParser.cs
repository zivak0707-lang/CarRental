using System.Globalization;
using CarRental.Core.Domain;
using CarRental.Core.Errors;

namespace CarRental.Core.Input;

/// <summary>
/// Барикада між «брудними» даними ззовні та сутностями:
/// перетворює рядки вводу на перевірену позицію бронювання.
/// </summary>
public static class ReservationDayParser
{
    /// <summary>Найбільша допустима тривалість прокату, діб.</summary>
    public const int MaxRentalDays = 365;

    /// <summary>Найбільша допустима довжина держномера, символів.</summary>
    public const int MaxPlateLength = 20;

    /// <summary>Найбільша допустима ставка за добу, грн.</summary>
    public const decimal MaxDailyRate = 100000m;

    private const string DateFormat = "dd.MM.yyyy";

    private const char FieldSeparator = ';';

    private const int FieldCount = 4;

    /// <summary>Українська культура: дати дд.мм.рррр, дробова частина через кому.</summary>
    private static CultureInfo Ukrainian => CultureInfo.GetCultureInfo("uk-UA");

    /// <summary>Розбирає рядок виду «номер;дата видачі;дата повернення;ставка».</summary>
    /// <param name="raw">Рядок ззовні, може бути порожнім.</param>
    /// <param name="line">Готова позиція або null, якщо рядок некоректний.</param>
    /// <returns>true, якщо рядок коректний.</returns>
    public static bool TryParse(string? raw, out ReservationDay? line)
    {
        line = null;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return false;
        }

        string[] parts = raw.Split(FieldSeparator);
        if (parts.Length != FieldCount)
        {
            return false;
        }

        Result<ReservationDay> parsed = Parse(parts[0], parts[1], parts[2], parts[3]);
        line = parsed.Value;
        return parsed.IsSuccess;
    }

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
        if (string.IsNullOrWhiteSpace(plate) || plate.Trim().Length > MaxPlateLength)
        {
            return Result.Fail<ReservationDay>(
                $"Державний номер — від 1 до {MaxPlateLength} символів.");
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

        if (end <= start || end.DayNumber - start.DayNumber > MaxRentalDays)
        {
            return Result.Fail<ReservationDay>(
                $"Дата повернення — пізніше за видачу, але не більш як на {MaxRentalDays} діб.");
        }

        if (!decimal.TryParse(dailyRate, NumberStyles.Number, Ukrainian, out decimal rate)
            || rate <= 0m
            || rate > MaxDailyRate)
        {
            return Result.Fail<ReservationDay>(
                $"Ставка — число від 0 до {MaxDailyRate} включно, наприклад 250,50.");
        }

        return Result.Ok(new ReservationDay(plate.Trim().ToUpperInvariant(), start, end, rate));
    }
}
