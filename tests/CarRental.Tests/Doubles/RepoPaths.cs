namespace CarRental.Tests.Doubles;

/// <summary>Шляхи всередині репозиторію: корінь шукаємо за файлом рішення.</summary>
public static class RepoPaths
{
    public static string Root()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "CarRental.sln")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Корінь не знайдено.");
    }

    public static string TempDirectory()
    {
        string name = Guid.NewGuid().ToString("N");
        string path = Path.Combine(Path.GetTempPath(), "carrental-tests", name);
        Directory.CreateDirectory(path);
        return path;
    }
}
