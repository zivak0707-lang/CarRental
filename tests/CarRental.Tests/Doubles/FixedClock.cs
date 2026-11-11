namespace CarRental.Tests.Doubles;

/// <summary>Тестовий дублер годинника: час стоїть, доки тест не зсуне його.</summary>
public sealed class FixedClock : TimeProvider
{
    private static readonly TimeZoneInfo _kyivWinter =
        TimeZoneInfo.CreateCustomTimeZone("Kyiv+2", TimeSpan.FromHours(2), "Kyiv+2", "Kyiv+2");

    private DateTimeOffset _now;

    public FixedClock(DateTimeOffset now)
    {
        _now = now;
    }

    public override TimeZoneInfo LocalTimeZone => _kyivWinter;

    public override DateTimeOffset GetUtcNow() => _now.ToUniversalTime();

    public void Advance(TimeSpan delta) => _now += delta;
}
