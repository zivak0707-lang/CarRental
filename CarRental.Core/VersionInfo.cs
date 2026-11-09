using System.Reflection;

namespace CarRental.Core;

/// <summary>Повертає версію збірки з Directory.Build.props.</summary>
public static class VersionInfo
{
    /// <summary>Читає версію зі збірки, без хешу коміту після знака +.</summary>
    /// <returns>Версія у форматі МАЖОР.МІНОР.ПАТЧ.</returns>
    public static string Current()
    {
        var assembly = typeof(VersionInfo).Assembly;
        var attribute = assembly.GetCustomAttribute<
            AssemblyInformationalVersionAttribute>();
        string raw = attribute?.InformationalVersion ?? "0.0.0";
        int plus = raw.IndexOf('+');
        return plus < 0 ? raw : raw[..plus];
    }
}
