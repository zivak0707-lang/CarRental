namespace CarRental.Core.Domain;

/// <summary>
/// Бронювання автомобілів: рядки з авто, кількістю діб і ставкою,
/// розрахунок підсумкової суми та керування станом бронювання.
/// </summary>
public class Booking
{
    /// <summary>Стан: нове бронювання.</summary>
    public const int StatusNew = 0;

    /// <summary>Стан: бронювання підтверджено.</summary>
    public const int StatusConfirmed = 1;

    /// <summary>Стан: автомобіль видано клієнтові.</summary>
    public const int StatusIssued = 2;

    /// <summary>Стан: бронювання скасовано.</summary>
    public const int StatusCancelled = 3;

    /// <summary>Ставка податку на додану вартість.</summary>
    private const decimal VatRate = 0.2m;

    /// <summary>Поріг суми для знижки постійному клієнту, грн.</summary>
    private const decimal RegularDiscountThreshold = 1000m;

    /// <summary>Частка знижки постійному клієнту.</summary>
    private const decimal RegularDiscountRate = 0.10m;

    /// <summary>Поріг суми для знижки на велике бронювання, грн.</summary>
    private const decimal LargeOrderThreshold = 5000m;

    /// <summary>Частка знижки на велике бронювання.</summary>
    private const decimal LargeOrderDiscountRate = 0.15m;

    /// <summary>Кількість рядків, більше якої діє гуртова знижка.</summary>
    private const int BulkLineCount = 10;

    /// <summary>Сума гуртової знижки, грн.</summary>
    private const decimal BulkDiscountAmount = 100m;

    /// <summary>Максимально допустима кількість рядків у бронюванні.</summary>
    private const int MaxLineCount = 100;

    /// <summary>Найкоротша допустима довжина імені клієнта.</summary>
    private const int MinCustomerNameLength = 3;

    private readonly List<string[]> _lines = new List<string[]>();

    /// <summary>
    /// Створює нове бронювання на вказаного клієнта.
    /// </summary>
    /// <param name="id">Ідентифікатор бронювання.</param>
    /// <param name="customerName">Ім'я клієнта.</param>
    public Booking(string id, string customerName)
    {
        Id = id;
        CustomerName = customerName;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// Ідентифікатор бронювання.
    /// </summary>
    public string Id { get; private set; }

    /// <summary>
    /// Ім'я клієнта.
    /// </summary>
    public string CustomerName { get; private set; }

    /// <summary>
    /// Поточний стан бронювання (одна з констант Status...).
    /// </summary>
    public int Status { get; private set; }

    /// <summary>
    /// Дата й час створення бронювання.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Шукає бронювання за ідентифікатором у списку.
    /// </summary>
    /// <param name="bookings">Список бронювань для пошуку.</param>
    /// <param name="bookingId">Ідентифікатор шуканого бронювання.</param>
    /// <returns>
    /// Знайдене бронювання або null, якщо збігів немає.
    /// </returns>
    public static Booking? FindById(List<Booking> bookings, string bookingId)
    {
        for (int i = 0; i < bookings.Count; i++)
        {
            if (bookings[i].Id == bookingId)
            {
                return bookings[i];
            }
        }

        return null;
    }

    /// <summary>
    /// Додає до бронювання рядок із автомобілем.
    /// </summary>
    /// <param name="plate">Державний номер автомобіля.</param>
    /// <param name="days">Кількість діб оренди.</param>
    /// <param name="dailyRate">Ставка за добу, грн.</param>
    public void AddLine(string plate, int days, decimal dailyRate)
    {
        string[] line = new string[3];
        line[0] = plate;
        line[1] = days.ToString();
        line[2] = dailyRate.ToString();
        _lines.Add(line);
    }

    /// <summary>
    /// Обчислює підсумкову суму до сплати з урахуванням знижок і ПДВ.
    /// </summary>
    /// <param name="isRegularCustomer">Ознака постійного клієнта.</param>
    /// <returns>
    /// Сума до сплати, заокруглена до двох знаків.
    /// </returns>
    public decimal CalculateTotal(bool isRegularCustomer)
    {
        decimal total = 0;
        int lineCount = 0;
        for (int i = 0; i < _lines.Count; i++)
        {
            int days = int.Parse(_lines[i][1]);
            decimal dailyRate = decimal.Parse(_lines[i][2]);
            total += days * dailyRate;
            lineCount = lineCount + 1;
        }

        // Порядок нарахування: спочатку знижка за сумою (постійному покупцю
        // або на велике замовлення, діє лише одна), потім гуртова знижка.
        if (isRegularCustomer && total > RegularDiscountThreshold)
        {
            total *= 1m - RegularDiscountRate;
        }
        else if (total > LargeOrderThreshold)
        {
            total *= 1m - LargeOrderDiscountRate;
        }

        if (lineCount > BulkLineCount)
        {
            total -= BulkDiscountAmount;
        }

        if (total < 0)
        {
            total = 0;
        }

        // ПДВ 20 % нараховується на суму вже після всіх знижок
        total += total * VatRate;
        return Math.Round(total, 2);
    }

    /// <summary>
    /// Змінює стан бронювання, якщо перехід дозволений
    /// правилами предметної області.
    /// </summary>
    /// <param name="newStatus">Цільовий стан бронювання.</param>
    /// <returns>
    /// true, якщо перехід виконано; false, якщо він заборонений.
    /// </returns>
    public bool TryChangeStatus(int newStatus)
    {
        if (Status == StatusNew && newStatus == StatusConfirmed)
        {
            Status = StatusConfirmed;
            return true;
        }

        if (Status == StatusConfirmed && newStatus == StatusIssued)
        {
            Status = StatusIssued;
            return true;
        }

        if (Status == StatusNew && newStatus == StatusCancelled)
        {
            Status = StatusCancelled;
            return true;
        }

        return false;
    }

    /// <summary>
    /// Перевіряє, чи бронювання коректне.
    /// </summary>
    /// <returns>
    /// true, якщо бронювання коректне; інакше false.
    /// </returns>
    public bool IsValid()
    {
        if (Id != null && Id != string.Empty
            && CustomerName != null
            && CustomerName.Length >= MinCustomerNameLength
            && _lines.Count > 0
            && _lines.Count < MaxLineCount
            && Status >= StatusNew
            && Status <= StatusCancelled)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Формує текстовий звіт за рядками бронювання та підсумковою сумою.
    /// </summary>
    /// <returns>Текст звіту з рядками й підсумком.</returns>
    public string BuildReport()
    {
        string report = string.Empty;
        for (int i = 0; i < _lines.Count; i++)
        {
            report = report + "Авто: " + _lines[i][0]
                + "; діб: " + _lines[i][1]
                + "; ставка: " + _lines[i][2]
                + "; сума: "
                + (int.Parse(_lines[i][1]) * decimal.Parse(_lines[i][2]))
                + "\n";
        }

        report = report + "Разом: " + CalculateTotal(false) + "\n";
        return report;
    }
}
