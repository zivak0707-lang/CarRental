using System.Globalization;
using System.Text;
using CarRental.Core.Domain;
using CarRental.Core.Rentals;

namespace CarRental.Core.Reporting;

/// <summary>Формування тексту звіту про прокат: чисті функції без виведення.</summary>
public static class RentalReport
{
    /// <summary>Будує текст звіту; результат залежить лише від аргументів.</summary>
    /// <param name="request">Запит, за яким виконано розрахунок.</param>
    /// <param name="result">Результат розрахунку.</param>
    /// <returns>Багаторядковий текст звіту.</returns>
    public static string TextOf(TotalRequest request, TotalResult result)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(result);
        CultureInfo culture = CultureInfo.InvariantCulture;
        StringBuilder text = new();
        text.AppendLine($"Прокат № {request.RentalId}");
        text.AppendLine($"Клієнт: {request.Customer.Name}");
        foreach (RentalLine line in request.Lines)
        {
            text.AppendLine($"{line.Plate} x {line.Days}");
        }

        text.AppendLine(string.Format(culture, "Разом: {0:F2}", result.Total));
        text.AppendLine($"Стан: {result.Status}");
        return text.ToString();
    }
}
