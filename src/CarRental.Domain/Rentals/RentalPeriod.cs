namespace CarRental.Domain.Rentals;

/// <summary>Запланований період оренди — напіввідкритий інтервал [Start; End).</summary>
public sealed record RentalPeriod
{
    /// <summary>Створює період.</summary>
    /// <param name="start">Запланований момент видачі.</param>
    /// <param name="end">Запланований момент повернення; пізніший за start.</param>
    public RentalPeriod(DateTimeOffset start, DateTimeOffset end)
    {
        ArgumentOutOfRangeException.ThrowIfLessThanOrEqual(end, start);
        Start = start;
        End = end;
    }

    /// <summary>Запланований момент видачі.</summary>
    public DateTimeOffset Start { get; }

    /// <summary>Запланований момент повернення.</summary>
    public DateTimeOffset End { get; }

    /// <summary>Чи перетинаються два періоди; спільна межа перетином не є.</summary>
    /// <param name="other">Інший період.</param>
    /// <returns>true, якщо в періодів є спільний момент.</returns>
    public bool Overlaps(RentalPeriod other)
    {
        ArgumentNullException.ThrowIfNull(other);
        return Start < other.End && other.Start < End;
    }

    /// <summary>Чи припадає календарний день на період.</summary>
    /// <param name="day">День за місцевим часом періоду.</param>
    /// <returns>true, якщо день лежить між датою видачі і датою повернення.</returns>
    public bool Covers(DateOnly day) =>
        DateOnly.FromDateTime(Start.DateTime) <= day && day < DateOnly.FromDateTime(End.DateTime);
}
