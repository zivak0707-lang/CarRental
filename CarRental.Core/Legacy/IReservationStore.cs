#pragma warning disable
// Успадкований код для аудиту SOLID (ЛР4, завдання 5); теку видаляємо в завданні 6.

namespace CarRental.Core.Legacy;

public interface IReservationStore
{
    void Add(LegacyReservation reservation);

    LegacyReservation GetById(int id);

    void SaveToFile(LegacyReservation reservation, string path);

    void SendEmail(string to, string body);

    string ExportCsv();

    void Backup(string folder);

    void PrintInvoice(LegacyReservation reservation);
}
