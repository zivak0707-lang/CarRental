using CarRental.Domain.Errors;

namespace CarRental.ConsoleApp;

/// <summary>Головне меню: запускає екрани і перетворює відмову домену на повідомлення.</summary>
internal sealed class MainMenu
{
    private readonly ConsoleIo _io;
    private readonly Dictionary<string, (string Title, Action Run)> _items;

    /// <summary>Створює меню.</summary>
    /// <param name="io">Ввід-вивід.</param>
    /// <param name="rentals">Екрани оренди (UC-01 – UC-04).</param>
    /// <param name="queries">Екрани пошуку і звіту (UC-05, UC-06).</param>
    public MainMenu(ConsoleIo io, RentalScreens rentals, QueryScreens queries)
    {
        _io = io;
        _items = new(StringComparer.Ordinal)
        {
            ["1"] = ("Забронювати автомобіль", rentals.Reserve),
            ["2"] = ("Видати автомобіль", rentals.PickUp),
            ["3"] = ("Закрити оренду", rentals.Close),
            ["4"] = ("Скасувати бронювання", rentals.Cancel),
            ["5"] = ("Знайти оренди", queries.Find),
            ["6"] = ("Звіт «Зайнятість автопарку»", queries.Occupancy),
        };
    }

    /// <summary>Крутить меню, доки не оберуть 0 або не закінчиться ввід.</summary>
    public void Run()
    {
        _io.Line("CarRental — облік прокату автомобілів філії");
        while (true)
        {
            _io.Line(string.Empty);
            foreach ((string key, (string title, _)) in _items)
            {
                _io.Line($"{key}. {title}");
            }

            _io.Line("0. Вихід");
            string? choice = _io.Ask("Оберіть пункт");
            if (choice is null or "0")
            {
                return;
            }

            RunScreen(choice);
        }
    }

    private void RunScreen(string choice)
    {
        if (!_items.TryGetValue(choice, out (string Title, Action Run) item))
        {
            _io.Line("Такого пункту немає; введіть число від 0 до 6.");
            return;
        }

        try
        {
            item.Run();
        }
        catch (DomainRuleException ex)
        {
            // Відмову вже записано в журнал сервісом на рівні Warning
            _io.Line($"Відхилено: {ex.Message}");
        }
    }
}
