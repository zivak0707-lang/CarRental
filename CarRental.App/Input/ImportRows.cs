namespace CarRental.App.Input;

/// <summary>Підрахунки над рядками, імпортованими ззовні.</summary>
internal static class ImportRows
{
    /// <summary>Рахує рядки, що містять додатну кількість діб.</summary>
    /// <param name="rows">Рядки з кількістю діб.</param>
    /// <returns>Кількість рядків із додатною кількістю діб.</returns>
    public static int CountValidDays(string[] rows)
    {
        ArgumentNullException.ThrowIfNull(rows);
        int valid = 0;
        foreach (string row in rows)
        {
            if (int.TryParse(row, out int days) && days > 0)
            {
                valid++;
            }
        }

        return valid;
    }
}
