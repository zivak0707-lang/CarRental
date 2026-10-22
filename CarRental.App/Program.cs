using System.Text;
using CarRental.App.Input;
using CarRental.Core.Abstractions;
using CarRental.Core.Domain;
using CarRental.Core.Errors;
using CarRental.Core.Notifications;
using CarRental.Core.Pricing;
using CarRental.Core.Reporting;
using CarRental.Core.Services;
using CarRental.Core.Storage;

Console.OutputEncoding = Encoding.UTF8;

// Верхній обробник: єдине місце, де дозволено catch (Exception)
try
{
    RunApplication();
    return 0;
}
catch (DomainRuleException ex)
{
    Console.Error.WriteLine($"Правило {ex.Rule}: {ex.Message}");
    return 1;
}
catch (IOException ex)
{
    Console.Error.WriteLine($"Помилка середовища: {ex.Message}");
    return 3;
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Непередбачений збій: {ex}");
    return 2;
}

static void RunApplication()
{
    // Барикада: поки ввід некоректний, пояснюємо помилку й питаємо знову
    ReservationDay line = ReadReservationDay();

    // Композиційний корінь: єдине місце, де створюються конкретні реалізації
    var policies = new Dictionary<string, IPricingPolicy>
    {
        ["regular"] = new DiscountPricingPolicy(0.05m),
        ["vip"] = new VipPricingPolicy(),
        ["staff"] = new DiscountPricingPolicy(0.30m),
    };

    var renter = new Renter(100, "Іваненко Петро", "petro@example.com") { IsRegular = true };
    string renterType = renter.IsRegular ? "regular" : "new";
    IPricingPolicy pricing = policies.TryGetValue(renterType, out IPricingPolicy? found)
        ? found
        : new StandardPricingPolicy();

    var repository = new InMemoryReservationRepository();
    var notifier = new TextWriterNotifier(Console.Out);
    var service = new ReservationService(repository, pricing, notifier);

    var reservation = new Reservation(1, renter.Id, DateOnly.FromDateTime(DateTime.Today));
    service.Place(reservation);
    service.AddLine(reservation.Id, line);
    reservation.Confirm();

    decimal total = service.TotalOf(reservation);
    Console.WriteLine($"Сума: {total:0.00}");
    service.Pay(reservation.Id, total + 25m);
    Console.Write(ReservationCsvReport.BuildCsv(repository.GetAll()));

    try
    {
        service.Cancel(reservation.Id, "клієнт передумав");
    }
    catch (DomainRuleException ex)
    {
        Console.WriteLine($"Відмова: {ex.Message}");
    }

    string[] importRows = { "3", "два", "-1", "7" };
    Console.WriteLine(
        $"Імпорт: коректних значень діб {ImportRows.CountValidDays(importRows)} "
        + $"з {importRows.Length}");
}

static ReservationDay ReadReservationDay()
{
    while (true)
    {
        Result<ReservationDay> parsed = ReservationDayParser.Parse(
            Ask("Держномер: "),
            Ask("Дата видачі (дд.мм.рррр): "),
            Ask("Дата повернення (дд.мм.рррр): "),
            Ask("Ставка за добу, грн: "));
        if (parsed.IsSuccess)
        {
            return parsed.Value!;
        }

        Console.WriteLine($"Помилка вводу: {parsed.Error}");
    }
}

static string Ask(string prompt)
{
    Console.Write(prompt);
    return Console.ReadLine() ?? throw new EndOfStreamException("Потік введення закрито.");
}
