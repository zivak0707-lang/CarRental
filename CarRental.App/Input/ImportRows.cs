using System.Globalization;
using CarRental.Core.Input;

namespace CarRental.App.Input;

/// <summary>Підрахунки над рядками, імпортованими ззовні.</summary>
internal static class ImportRows
{
    /// <summary>Рахує рядки з кількістю діб у допустимих межах.</summary>
    /// <param name="rows">Рядки з кількістю діб.</param>
    /// <returns>Кількість рядків із коректною кількістю діб.</returns>
    public static int CountValidDays(string[] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, NumberStyles.Integer, CultureInfo.InvariantCulture, out int days)
                && days is > 0 and <= ReservationDayParser.MaxRentalDays)
            {
                valid++;
            }
        }

        return valid;
    }
}
