namespace CarRental.App.Input;

/// <summary>Підрахунки над рядками, імпортованими ззовні.</summary>
internal static class ImportRows
{
    /// <summary>ЛР5, стан before: try/catch замість звичайної умови.</summary>
    /// <param name="rows">Рядки з кількістю діб.</param>
    /// <returns>Кількість рядків із додатною кількістю діб.</returns>
    public static int CountValidDays(string[] rows)
    {
        var valid = 0;
        foreach (var row in rows)
        {
            try
            {
                var days = int.Parse(row);
                if (days > 0)
                {
                    valid++;
                }
            }
            catch (FormatException)
            {
                // не число — пропускаємо
            }
        }

        return valid;
    }
}
