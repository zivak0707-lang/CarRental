#pragma warning disable
// Успадкований код для аудиту SOLID (ЛР4, завдання 5); теку видаляємо в завданні 6.
// У методичці цих двох класів немає — це мінімальні заглушки, щоб клас-Бог компілювався.

using System.Text.Json;

namespace CarRental.Core.Legacy;

public class FileReservationStore : IReservationStore
{
    public void SaveToFile(LegacyReservation reservation, string path)
    {
        File.AppendAllText(path, JsonSerializer.Serialize(reservation) + Environment.NewLine);
    }

    public void Add(LegacyReservation reservation) => throw new NotSupportedException();

    public LegacyReservation GetById(int id) => throw new NotSupportedException();

    public void SendEmail(string to, string body) => throw new NotSupportedException();

    public string ExportCsv() => throw new NotSupportedException();

    public void Backup(string folder) => throw new NotSupportedException();

    public void PrintInvoice(LegacyReservation reservation) => throw new NotSupportedException();
}

public class SmtpMailer
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine("SMTP → " + to + ": " + subject + " " + body);
    }
}
