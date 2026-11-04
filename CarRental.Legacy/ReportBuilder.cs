using System.Text;

namespace CarRental.Legacy;

public sealed class ReportBuilder
{
    public string Build(RentalRequest req, decimal discount, decimal shipping, decimal total)
    {
        var sb = new StringBuilder();
        sb.Append("Документ #").Append(req.ReservationId).Append('\n');
        sb.Append("Клієнт: ").Append(req.Renter.Name).Append('\n');
        foreach (CarLine line in req.Lines!)
        {
            sb.Append(Line(line, req.Currency));
        }

        sb.Append(Money("Знижка", discount, req.Currency));
        sb.Append(Money("Доставка", shipping, req.Currency));
        sb.Append(Money("Разом", total, req.Currency));
        return sb.ToString();
    }

    private static string Line(CarLine line, string currency) =>
        line.Plate + " x" + line.Days + " = " +
        (line.Days * line.DailyRate).ToString("0.00") + " " + currency + "\n";

    private static string Money(string label, decimal value, string currency) =>
        label + ": " + value.ToString("0.00") + " " + currency + "\n";
}
