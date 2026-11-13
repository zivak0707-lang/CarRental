using CarRental.Infrastructure.Logging;
using CarRental.Tests.Doubles;
using Microsoft.Extensions.Logging;

namespace CarRental.Tests.Infrastructure;

public class FileLoggerTests
{
    [Fact]
    public void LogWarning_Message_WritesTimeLevelSourceAndText()
    {
        string path = Path.Combine(RepoPaths.TempDirectory(), "logs", "app.log");
        using var provider = new FileLoggerProvider(path, new FixedClock(Sample.Now));
        ILogger log = provider.CreateLogger("CarRental.Application.Rentals.HandoverService");

        log.LogWarning("UC-02 відхилено: {Reason}", "договору немає");

        Assert.Equal(
            "2026-11-10 10:00:00 | Warning | HandoverService | UC-02 відхилено: договору немає",
            File.ReadAllText(path).TrimEnd());
    }
}
