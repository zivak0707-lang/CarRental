namespace CarRental.Core.Errors;

/// <summary>
/// Порушено правило предметної області. Rule — ідентифікатор правила,
/// за яким помилку можна знайти в журналі (наприклад, reservation.payable).
/// </summary>
public class DomainRuleException : Exception
{
    /// <summary>Створює виняток для порушеного правила.</summary>
    /// <param name="rule">Ідентифікатор правила, не порожній.</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    public DomainRuleException(string rule, string message)
        : base(message)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }

    /// <summary>Створює виняток, що загортає технічну причину.</summary>
    /// <param name="rule">Ідентифікатор правила, не порожній.</param>
    /// <param name="message">Повідомлення українською з конкретними значеннями.</param>
    /// <param name="innerException">Початковий виняток; його стек зберігається.</param>
    public DomainRuleException(string rule, string message, Exception innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rule);
        Rule = rule;
    }

    /// <summary>Ідентифікатор порушеного правила.</summary>
    public string Rule { get; }
}
