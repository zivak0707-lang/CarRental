using CarRental.Core.Domain;
using CarRental.Core.Input;

namespace CarRental.Tests;

public class ReservationDayParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("AA1234KI;02.11.2026;04.11.2026")]
    [InlineData(";02.11.2026;04.11.2026;250,50")]
    [InlineData("AAAAAAAAAAAAAAAAAAAAA;02.11.2026;04.11.2026;250,50")]
    [InlineData("AA1234KI;2026-11-02;04.11.2026;250,50")]
    [InlineData("AA1234KI;02.11.2026;02.11.2026;250,50")]
    [InlineData("AA1234KI;02.11.2026;03.11.2027;250,50")]
    [InlineData("AA1234KI;02.11.2026;04.11.2026;0")]
    [InlineData("AA1234KI;02.11.2026;04.11.2026;-1")]
    [InlineData("AA1234KI;02.11.2026;04.11.2026;100000,01")]
    [InlineData("AA1234KI;02.11.2026;04.11.2026;abc")]
    public void TryParse_InvalidInput_ReturnsFalse(string raw)
    {
        // Act
        bool ok = ReservationDayParser.TryParse(raw, out ReservationDay? line);

        // Assert
        Assert.False(ok);
        Assert.Null(line);
    }

    [Fact]
    public void TryParse_ValidInput_ReturnsNormalizedLine()
    {
        // Arrange
        string raw = " aa1234ki ;02.11.2026;04.11.2026;250,50";

        // Act
        bool ok = ReservationDayParser.TryParse(raw, out ReservationDay? line);

        // Assert
        Assert.True(ok);
        Assert.Equal("AA1234KI", line!.Plate);
        Assert.Equal(2, line.Days);
        Assert.Equal(250.50m, line.DailyRate);
    }
}
