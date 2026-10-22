#pragma warning disable
// Успадкований код для аудиту SOLID (ЛР4, завдання 5); теку видаляємо в завданні 6.

namespace CarRental.Core.Legacy;

public class ReservationManager
{
    private readonly FileReservationStore _store = new FileReservationStore();
    private readonly SmtpMailer _mailer = new SmtpMailer();

    public decimal Place(LegacyReservation reservation, string customerType)
    {
        decimal total = 0m;
        foreach (var line in reservation.Lines)
        {
            total += line.DailyRate * line.Days;
        }

        if (customerType == "regular")
        {
            total = total * 0.95m;
        }
        else if (customerType == "vip")
        {
            total = total * 0.90m;
        }
        else if (customerType == "staff")
        {
            total = total * 0.70m;
        }

        if (total > 1000m)
        {
            total = total - 50m;
        }

        reservation.Total = total;
        reservation.Status = 1;
        _store.SaveToFile(reservation, "reservations.json");
        _mailer.Send(reservation.Email, "Reservation " + reservation.Id, "" + total);
        return total;
    }

    public string BuildCsvReport(List<LegacyReservation> reservations)
    {
        var text = "id;total;status" + Environment.NewLine;
        foreach (var r in reservations)
        {
            text += r.Id + ";" + r.Total + ";" + r.Status + Environment.NewLine;
        }

        File.WriteAllText("report.csv", text);
        Console.WriteLine("Звіт збережено");
        return text;
    }

    public void Archive(LegacyReservation reservation)
    {
        LegacyReservation archived = new ArchivedReservation(reservation.Id);
        foreach (var line in reservation.Lines)
        {
            archived.AddLine(line);
        }

        _store.SaveToFile(archived, "archive.json");
    }
}
