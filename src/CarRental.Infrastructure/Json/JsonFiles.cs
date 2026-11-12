using System.Text.Json;

namespace CarRental.Infrastructure.Json;

/// <summary>Читання й запис файлів JSON у кодуванні UTF-8.</summary>
internal static class JsonFiles
{
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = true,
    };

    /// <summary>Читає перелік записів; помилку формату перетворює на виняток даних.</summary>
    /// <typeparam name="T">Тип запису файлу.</typeparam>
    /// <param name="path">Шлях до файлу.</param>
    /// <returns>Записи файлу.</returns>
    public static List<T> ReadList<T>(string path)
    {
        try
        {
            return JsonSerializer.Deserialize<List<T>>(File.ReadAllText(path), _options) ?? [];
        }
        catch (JsonException ex)
        {
            throw new InvalidDataException($"Файл {path} пошкоджено: {ex.Message}", ex);
        }
    }

    /// <summary>Записує перелік через тимчасовий файл, щоб збій не зіпсував дані.</summary>
    /// <typeparam name="T">Тип запису файлу.</typeparam>
    /// <param name="path">Шлях до файлу.</param>
    /// <param name="items">Записи.</param>
    public static void WriteList<T>(string path, IEnumerable<T> items)
    {
        string temp = path + ".tmp";
        File.WriteAllText(temp, JsonSerializer.Serialize(items, _options));
        File.Move(temp, path, overwrite: true);
    }
}
