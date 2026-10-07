using System.Text;
using CarRental.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

var booking = new Booking("B-1001", "Іваненко");
booking.AddLine("AA1234AA", 3, 250m);
booking.AddLine("KA5678KA", 12, 90m);
Console.WriteLine(booking.CalculateTotal(false));
Console.WriteLine(booking.CalculateTotal(true));
Console.WriteLine(booking.IsValid());
Console.WriteLine(booking.TryChangeStatus(1));
Console.Write(booking.BuildReport());