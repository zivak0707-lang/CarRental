using System.Globalization;
using CarRental.Legacy;

namespace CarRental.Legacy.Tests;

public class LegacyCharacterizationTests
{
    public LegacyCharacterizationTests()
    {
        // формат чисел залежить від культури середовища, тому фіксуємо її явно
        CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;
    }

    [Theory]
    [InlineData(null, DocumentState.New, "a@b.c", "ERR: null")]
    [InlineData("empty", DocumentState.New, "a@b.c", "ERR: empty")]
    [InlineData("two", DocumentState.Draft, "a@b.c", "ERR: state")]
    [InlineData("two", DocumentState.New, "no-mail", "ERR: mail")]
    public void Handle_BadInput_ReturnsErrorCode(
        string? kindOfLines, DocumentState state, string mail, string expected)
    {
        List<CarLine>? lines = kindOfLines switch
        {
            "two" => TwoLines(),
            "empty" => new List<CarLine>(),
            _ => null,
        };
        var sut = new LegacyRentalProcessor();

        string actual = sut.Handle(new RentalRequest(
            1001, Ivan(RenterKind.Regular, 0, mail), lines, state, "UAH", false));

        Assert.Equal(expected, actual);
    }

    [Fact]
    public void Handle_VipReservation_ReturnsExactReport()
    {
        var sut = new LegacyRentalProcessor();

        string report = sut.Handle(new RentalRequest(
            1001, Ivan(RenterKind.Vip, 3, "a@b.c"), TwoLines(), DocumentState.New, "UAH", false));

        string expected =
            "Документ #1001\n" +
            "Клієнт: Іван\n" +
            "AA1234KI x2 = 300.00 UAH\n" +
            "KA5678BB x1 = 400.00 UAH\n" +
            "Знижка: 105.00 UAH\n" +
            "Доставка: 60.00 UAH\n" +
            "Разом: 655.00 UAH\n";
        Assert.Equal(expected, report);
    }

    [Theory]
    [InlineData(RenterKind.Regular, 0, 760)]
    [InlineData(RenterKind.Vip, 0, 655)]
    [InlineData(RenterKind.Staff, 0, 550)]
    [InlineData(RenterKind.Regular, 11, 725)]
    public void Preview_ByKind_ReturnsTotal(RenterKind kind, int done, decimal expected)
    {
        var sut = new LegacyRentalProcessor();

        decimal total = sut.Preview(Ivan(kind, done, "a@b.c"), TwoLines());

        Assert.Equal(expected, total);
    }

    [Fact]
    public void Preview_TotalExactly1000_NoShipping()
    {
        var sut = new LegacyRentalProcessor();
        var lines = new List<CarLine>
        {
            new CarLine { Plate = "AA1234KI", Days = 1, DailyRate = 1000m },
        };

        decimal total = sut.Preview(Ivan(RenterKind.Regular, 0, "a@b.c"), lines);

        Assert.Equal(1000m, total);
    }

    [Fact]
    public void Preview_VipLargeReservation_DiscountCappedAt500()
    {
        var sut = new LegacyRentalProcessor();
        var lines = new List<CarLine>
        {
            new CarLine { Plate = "AA1234KI", Days = 10, DailyRate = 400m },
        };

        decimal total = sut.Preview(Ivan(RenterKind.Vip, 0, "a@b.c"), lines);

        Assert.Equal(3500m, total);
    }

    [Fact]
    public void DescribeClient_Vip_BuildsCaption()
    {
        var renter = new Renter
        {
            Name = "  іван  ",
            Email = "IVAN@MAIL.COM",
            Kind = RenterKind.Vip,
            DoneCount = 12,
            SinceUtc = new DateTime(2020, 1, 1),
        };

        // метод залежить від системного годинника, тому очікування обчислюємо, а не вписуємо
        int years = DateTime.Now.Year - 2020;
        string expected = "ІВАН [VIP] [ЛОЯЛЬНИЙ] <ivan@mail.com> стаж " + years;

        string actual = new LegacyRentalProcessor().DescribeClient(renter);

        Assert.Equal(expected, actual);
    }

    private static Renter Ivan(RenterKind kind, int done, string mail) => new()
    {
        Id = 7,
        Name = "Іван",
        Email = mail,
        Kind = kind,
        DoneCount = done,
    };

    private static List<CarLine> TwoLines() => new()
    {
        new CarLine { Plate = "AA1234KI", Days = 2, DailyRate = 150m },
        new CarLine { Plate = "KA5678BB", Days = 1, DailyRate = 400m },
    };
}
