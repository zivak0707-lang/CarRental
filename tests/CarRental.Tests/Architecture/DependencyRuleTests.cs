using CarRental.Application.Rentals;
using CarRental.Domain.Rentals;

namespace CarRental.Tests.Architecture;

/// <summary>Правило залежностей: стрілки спрямовані всередину, домен не знає ні про кого.</summary>
public class DependencyRuleTests
{
    [Fact]
    public void Domain_ReferencesNoProjectOrFramework()
    {
        string[] names = References(typeof(Rental));

        Assert.Empty(names.Where(n => n.StartsWith("CarRental", StringComparison.Ordinal)
            || n.StartsWith("Microsoft.Extensions", StringComparison.Ordinal)
            || n.StartsWith("System.Text.Json", StringComparison.Ordinal)));
    }

    [Fact]
    public void Application_DoesNotReferenceInfrastructure()
    {
        string[] names = References(typeof(ReservationService));

        Assert.Contains("CarRental.Domain", names);
        Assert.DoesNotContain("CarRental.Infrastructure", names);
    }

    private static string[] References(Type type) =>
        type.Assembly.GetReferencedAssemblies().Select(a => a.Name!).ToArray();
}
