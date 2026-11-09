using CarRental.Core;
using CarRental.Core.Reporting;

namespace CarRental.Tests;

public class ReportBuilderTests
{
    [Fact]
    public void Build_TwoRows_StartsWithUpperCaseTitle()
    {
        // Arrange
        ReportBuilder builder = new() { Title = "Звіт" };
        string[] rows = { "перший", "другий" };

        // Act
        string text = builder.Build(rows);

        // Assert
        Assert.StartsWith("ЗВІТ", text);
        Assert.Contains("другий", text);
    }

    [Fact]
    public void VersionInfo_Current_ReturnsVersionWithoutCommitHash()
    {
        // Act
        string version = VersionInfo.Current();

        // Assert
        Assert.Equal("0.8.0", version);
    }
}
