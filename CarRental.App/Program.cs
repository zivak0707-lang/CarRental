using System.Text;
using CarRental.Core.Domain;

Console.OutputEncoding = Encoding.UTF8;

var z = new bron("B-1001", "Іваненко");
z.Add("AA1234AA", 3, 250m);
z.Add("KA5678KA", 12, 90m);
Console.WriteLine(z.ProcessData(false));
Console.WriteLine(z.ProcessData(true));
Console.WriteLine(z.Ck());
Console.WriteLine(z.Chg(1));
Console.Write(z.Rep());