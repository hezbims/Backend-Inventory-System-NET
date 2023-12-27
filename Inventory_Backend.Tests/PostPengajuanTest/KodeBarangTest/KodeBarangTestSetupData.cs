using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Moq;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

public class KodeBarangTestSetupData
{
    public TimeProvider MockTimeProvider { get; set; }
    public User Admin { get; set; }
    public User NonAdmin { get; set; }
    public Pengaju Pemasok { get; set; }
    public Pengaju Group { get; set; }
    
    public List<Barang> Barangs { get; set; }
    public MyDbFixture Fixture { get; set; }
    

    public KodeBarangTestSetupData(MyDbFixture fixture)
    {
        var mockDateTime = new DateTimeOffset(
            year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0,
            offset: TimeSpan.Zero
        );
        var mockTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
        var mockTimeProvider = new Mock<TimeProvider>();
        mockTimeProvider.Setup(p => p.GetUtcNow()).Returns(mockDateTime);
        mockTimeProvider.Setup(p => p.LocalTimeZone).Returns(mockTimeZone);
        MockTimeProvider = mockTimeProvider.Object;
        
        
        Fixture = fixture;

        using var db = Fixture.CreateContext();

        (Admin, NonAdmin, _) = db.SeedThreeUser();
        (Pemasok, Group) = db.SeedDuaPengaju();
        Barangs = db.SeedBarang20WithQuantity20();
    }
}