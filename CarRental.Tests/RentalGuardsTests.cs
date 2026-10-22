using CarRental.Core.Domain;
using CarRental.Core.Rentals;

namespace CarRental.Tests;

public class RentalGuardsTests
{
    [Fact]
    public void EnsureLinesValid_NoLines_ThrowsArgument()
    {
        Assert.Throws<ArgumentException>(() =>
            RentalGuards.EnsureLinesValid(new List<RentalLine>()));
    }

    [Fact]
    public void EnsureLineValid_ZeroDays_ThrowsRange()
    {
        RentalLine line = new() { Plate = "AA1234KI", Days = 0, DailyRate = 10m };

        Assert.Throws<ArgumentOutOfRangeException>(
            () => RentalGuards.EnsureLineValid(line));
    }
}
