using CarRental.Core.Storage;

namespace CarRental.Tests;

public class FileAuditLogTests
{
    [Fact]
    public void Dispose_AfterExceptionInUsing_ReleasesFile()
    {
        string path = Path.GetTempFileName();

        Assert.Throws<InvalidOperationException>(() => WriteThenFail(path));

        string text = File.ReadAllText(path);
        File.Delete(path);
        Assert.Contains("reservation.paid;1", text);
        Assert.False(File.Exists(path));
    }

    private static void WriteThenFail(string path)
    {
        using var audit = new FileAuditLog(path, TimeProvider.System);
        audit.Write("reservation.paid", 1);
        throw new InvalidOperationException("збій після запису");
    }
}
