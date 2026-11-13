using CarRental.Domain.Cars;
using CarRental.Domain.Pricing;
using CarRental.Domain.Rentals;
using CarRental.Infrastructure.Json;
using CarRental.Tests.Doubles;

namespace CarRental.Tests.Infrastructure;

/// <summary>Інфраструктура поза межею покриття, але збереження й читання перевіряємо.</summary>
public class JsonStorageTests
{
    [Fact]
    public void JsonRentalRepository_SaveThenLoad_RestoresClosedRental()
    {
        string path = Path.Combine(RepoPaths.TempDirectory(), "rentals.json");
        var rental = Sample.Active(1001, "AA1001AA", Sample.Now);
        var calculator = new RentalChargeCalculator(new StandardLateFeePolicy());
        rental.Close(Sample.Now.AddDays(1), calculator);
        var first = new JsonRentalRepository(path);
        first.Add(rental);
        first.Save();

        var loaded = new JsonRentalRepository(path).Find(1001);

        Assert.Equal(RentalStatus.Closed, loaded!.Status);
        Assert.Equal(rental.Terms, loaded.Terms);
        Assert.Equal(rental.Charge, loaded.Charge);
        Assert.Equal(1002, new JsonRentalRepository(path).NextNumber());
    }

    [Fact]
    public void JsonFleetCatalog_RepositoryData_LoadsAllDirectories()
    {
        var catalog = new JsonFleetCatalog(Path.Combine(RepoPaths.Root(), "data"));

        Assert.Equal(60, catalog.Cars.Count);
        Assert.Equal(900m, catalog.TariffFor(CarClass.Economy).DailyRate);
        Assert.NotNull(catalog.FindCustomer(120));
    }

    [Fact]
    public void JsonFleetCatalog_UnknownCarClass_ThrowsInvalidData()
    {
        string dir = RepoPaths.TempDirectory();
        string cars = """[{"plate":"AA1","model":"Bus","class":"Bus"}]""";
        File.WriteAllText(Path.Combine(dir, "cars.json"), cars);

        Assert.Throws<InvalidDataException>(() => new JsonFleetCatalog(dir));
    }
}
