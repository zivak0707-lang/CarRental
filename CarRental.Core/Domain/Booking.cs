using System;
using System.Collections.Generic;
using System.Linq;

namespace CarRental.Core.Domain;

// клас
public class Booking
{
    private readonly List<string[]> _lines = new List<string[]>();

    // public string prim; // примітка, поки не треба

    // конструктор
    public Booking(string id, string customerName)
    {
        Id = id; // ставимо id
        CustomerName = customerName; // ставимо cl
        CreatedAt = DateTime.Now; // ставимо дату
    }

    public string Id { get; private set; }

    public string CustomerName { get; private set; }

    public int Status { get; private set; } // 0-нова,1-підтв,2-видано,3-скасов

    public DateTime CreatedAt { get; private set; }

    // метод додавання
    public void AddLine(string plate, int days, decimal dailyRate)
    {
        string[] line = new string[3];
        line[0] = plate;
        line[1] = days.ToString();
        line[2] = dailyRate.ToString();
        _lines.Add(line); // додаємо t у ln
    }

    // ProcessData
    public decimal CalculateTotal(bool isRegularCustomer)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int days = int.Parse(_lines[i][1]);
            decimal dailyRate = decimal.Parse(_lines[i][2]);
            total = total + days * dailyRate; // додаємо до суми
            lineCount = lineCount + 1; // збільшуємо kolvo на одиницю
        }

        // if (sum1 > 500) { sum1 = sum1 - 50; } // стара знижка
        if (isRegularCustomer == true && total > 1000)
        {
            total = total * 0.9m;
        }
        else if (total > 5000)
        {
            total = total * 0.85m;
        }
        else
        {
            total = total;
        }

        if (lineCount > 10)
        {
            total = total - 100;
        }

        if (total < 0)
        {
            total = 0;
        }

        total = total + total * 0.2m;
        return Math.Round(total, 2); // повертаємо sum1
    }

    // міняємо статус
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == 0 && newStatus == 1)
        {
            Status = 1;
            return true;
        }

        if (Status == 1 && newStatus == 2)
        {
            Status = 2;
            return true;
        }

        if (Status == 0 && newStatus == 3)
        {
            Status = 3;
            return true;
        }

        return false; // не можна
    }

    // перевірка
    public bool IsValid()
    {
        if (Id != null && Id != ""
            && CustomerName != null
            && CustomerName.Length > 2
            && _lines.Count > 0
            && _lines.Count < 100
            && Status >= 0
            && Status <= 3)
        {
            return true;
        }

        return false;
    }

    // звіт
    public string BuildReport()
    {
        string report = "";
        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Авто: " + _lines[i][0]
                + "; діб: " + _lines[i][1]
                + "; ставка: " + _lines[i][2]
                + "; сума: "
                + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2]))
                + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report; // повертаємо s
    }

    // пошук
    public static Booking FindById(List<Booking> bookings, string bookingId)
    {
        for (int i = 0; i < bookings.Count; i++)
        {
            if (bookings[i].Id == bookingId)
            {
                return bookings[i];
            }
        }

        return null;
    }
}
