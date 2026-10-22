#pragma warning disable
// Успадкований код для аудиту SOLID (ЛР4, завдання 5); теку видаляємо в завданні 6.

namespace CarRental.Core.Legacy;

public class LegacyReservationLine
{
    public string Plate { get; set; } = "";

    public int Days { get; set; }

    public decimal DailyRate { get; set; }
}

public class LegacyReservation
{
    public int Id { get; set; }

    public int Status { get; set; }

    public decimal Total { get; set; }

    public string Email { get; set; } = "";

    public List<LegacyReservationLine> Lines { get; set; } = new();

    public virtual void AddLine(LegacyReservationLine line) => Lines.Add(line);
}

public class ArchivedReservation : LegacyReservation
{
    public ArchivedReservation(int id) => Id = id;

    public override void AddLine(LegacyReservationLine line)
    {
        throw new NotSupportedException("Архів змінювати не можна");
    }
}
