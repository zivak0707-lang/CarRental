using System.Globalization;
using System.Text;
using CarRental.Core.Domain;

namespace CarRental.Core.Reporting;

/// <summary>Форматування звіту про бронювання у CSV: лише будує рядок, нічого не пише.</summary>
public static class ReservationCsvReport
{
    /// <summary>Будує CSV: номер, стан, кількість діб і вартість без знижок.</summary>
    /// <param name="reservations">Бронювання для звіту.</param>
    /// <returns>Текст CSV із рядком заголовка.</returns>
    public static string BuildCsv(IEnumerable<Reservation> reservations)
    {
        ArgumentNullException.ThrowIfNull(reservations);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine("id;status;days;amount");
        foreach (Reservation reservation in reservations)
        {
            int days = reservation.Lines.Sum(line => line.Days);
            decimal amount = reservation.Lines.Sum(line => line.Amount);
            text.AppendLine(string.Format(
                culture,
                "{0};{1};{2};{3:F2}",
                reservation.Id,
                reservation.Status,
                days,
                amount));
        }

        return text.ToString();
    }
}
