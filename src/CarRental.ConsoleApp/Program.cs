using System.Text;
using CarRental.Application.Fleet;
using CarRental.Application.Rentals;
using CarRental.ConsoleApp;
using CarRental.Domain.Pricing;
using CarRental.Infrastructure.Json;
using CarRental.Infrastructure.Logging;
using Microsoft.Extensions.Logging;

Console.OutputEncoding = Encoding.UTF8;
Console.InputEncoding = Encoding.UTF8;
string dataDirectory = args.Length > 0 ? args[0] : "data";
TimeProvider clock = TimeProvider.System;
using ILoggerFactory logs = LoggerFactory.Create(
    b => b.AddProvider(new FileLoggerProvider(Path.Combine("logs", "carrental.log"), clock)));
ILogger log = logs.CreateLogger("Program");
try
{
    // Композиційний корінь: єдине місце, де створюються конкретні реалізації
    var catalog = new JsonFleetCatalog(dataDirectory);
    var rentals = new JsonRentalRepository(Path.Combine(dataDirectory, "rentals.json"));
    var calculator = new RentalChargeCalculator(new StandardLateFeePolicy());
    var io = new ConsoleIo(Console.In, Console.Out);
    var rentalScreens = new RentalScreens(
        io,
        new ReservationService(rentals, catalog, clock, logs.CreateLogger<ReservationService>()),
        new HandoverService(rentals, calculator, clock, logs.CreateLogger<HandoverService>()),
        clock);
    var queryScreens = new QueryScreens(
        io,
        new FleetQueryService(rentals, catalog, logs.CreateLogger<FleetQueryService>()),
        catalog,
        clock);
    new MainMenu(io, rentalScreens, queryScreens).Run();
    return 0;
}
catch (Exception ex) when (ex is IOException or InvalidDataException or UnauthorizedAccessException)
{
    log.LogError(ex, "Дані не завантажено з теки {Directory}", dataDirectory);
    Console.Error.WriteLine($"Помилка даних: {ex.Message}");
    return 1;
}
