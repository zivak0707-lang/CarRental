using CarRental.Core.Domain;

namespace CarRental.Core.Legacy;

/// <summary>
/// Вихідний метод-монстр: універсальний розрахунок підсумку прокату з перевіркою, знижками,
/// зміною стану й друком звіту в одному місці.
/// </summary>
public static class LegacyProcessor
{
    /// <summary>
    /// Перевіряє вхід, рахує підсумок прокату, змінює стан і друкує звіт.
    /// </summary>
    /// <param name="rentalId">Номер прокату.</param>
    /// <param name="customerName">Ім'я клієнта.</param>
    /// <param name="customerEmail">Пошта клієнта.</param>
    /// <param name="isRegular">Ознака постійного клієнта.</param>
    /// <param name="lines">Рядки прокату.</param>
    /// <param name="createdAt">Дата створення прокату.</param>
    /// <param name="couponCode">Код купона.</param>
    /// <param name="status">Поточний стан прокату: New, Confirmed, Active або Cancelled.</param>
    /// <param name="printToConsole">Чи друкувати звіт у консоль.</param>
    /// <param name="deliveryPrice">Вартість подачі автомобіля, грн.</param>
    /// <param name="newStatus">Стан прокату після розрахунку.</param>
    /// <returns>Підсумок прокату або -1, якщо вхідні дані некоректні.</returns>
    public static decimal Process(
        int rentalId,
        string customerName,
        string customerEmail,
        bool isRegular,
        List<RentalLine> lines,
        DateOnly createdAt,
        string couponCode,
        string status,
        bool printToConsole,
        decimal deliveryPrice,
        out string newStatus)
    {
        newStatus = status;
        decimal total = 0;
        if (rentalId <= 0)
        {
            Console.WriteLine("Помилка: номер прокату");
            return -1;
        }

        if (customerName == null || customerName == string.Empty)
        {
            Console.WriteLine("Помилка: немає клієнта");
            return -1;
        }

        if (customerEmail == null || !customerEmail.Contains('@'))
        {
            Console.WriteLine("Помилка: пошта клієнта");
            return -1;
        }

        if (lines == null || lines.Count == 0)
        {
            Console.WriteLine("Помилка: немає позицій");
            return -1;
        }

        if (status != "New" && status != "Confirmed" &&
            status != "Active" && status != "Cancelled")
        {
            Console.WriteLine("Помилка: невідомий стан");
            return -1;
        }

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].Days <= 0)
            {
                Console.WriteLine("Помилка: кількість діб");
                return -1;
            }

            if (lines[i].DailyRate < 0)
            {
                Console.WriteLine("Помилка: ставка");
                return -1;
            }

            decimal sum = lines[i].Days * lines[i].DailyRate;
            if (lines[i].Days >= 10)
            {
                sum = sum * 0.95m;
            }

            total = total + sum;
        }

        if (isRegular)
        {
            total = total * 0.93m;
        }

        if (total > 5000)
        {
            total = total * 0.9m;
        }
        else if (total > 2000)
        {
            total = total * 0.95m;
        }

        if (couponCode == "SALE10")
        {
            total -= total * 0.1m;
        }
        else if (couponCode == "MINUS200" && total > 1000)
        {
            total = total - 200;
        }

        if (createdAt.DayOfWeek == DayOfWeek.Sunday)
        {
            total = total * 0.98m;
        }

        if (total < 1000)
        {
            total = total + deliveryPrice;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = Math.Round(total, 2);
        if (status == "New" && total > 0)
        {
            newStatus = "Confirmed";
        }
        else if (status == "Confirmed")
        {
            newStatus = "Active";
        }
        else if (status == "Cancelled")
        {
            newStatus = "Cancelled";
            total = 0;
        }

        if (printToConsole)
        {
            Console.WriteLine("Прокат № " + rentalId);
            Console.WriteLine("Клієнт: " + customerName);
            Console.WriteLine("Пошта: " + customerEmail);
            for (int i = 0; i < lines.Count; i++)
            {
                Console.WriteLine(lines[i].Plate + " x " + lines[i].Days);
            }

            Console.WriteLine("Разом: " + total);
            Console.WriteLine("Стан: " + newStatus);
        }

        return total;
    }
}
