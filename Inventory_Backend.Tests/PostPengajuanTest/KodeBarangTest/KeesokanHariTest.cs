using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class KeesokanHariTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;

    public KeesokanHariTest(
        TestWebAppFactory webApp,
        ITestOutputHelper output)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(output);

        using var db = _webApp.GetDbContext();
        new CompleteTestSeeder(db: db).Run();
        
    }
    [Fact]
    public async Task Test_Ketika_Mulai_Mengajukan_DiKeesokan_Hari_Maka_Urutan_Transaksi_Mulai_Dari_1_Lagi()
    {
        using var db = _webApp.GetDbContext();
        var client = _webApp.GetAuthorizedClient(isAdmin: true);
        
        TestTimeProvider.Instance.AddDays(1);
        var response = await client.PostAsJsonAsync(
            "/api/pengajuan/add",
            new CreatePengajuanRequest
            {
                IdPegaju = db.Pengajus.First(pengaju => pengaju.IsPemasok).Id,
                ListBarangAjuan = new List<BarangAjuanRequest>
                {
                    new BarangAjuanRequest
                    {
                        IdBarang = db.Barangs.First().Id,
                        Keterangan = "",
                        Quantity = 1
                    }
                }
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        db.Pengajuans
            .OrderBy(pengajuan => pengajuan.WaktuPengajuan)
            .Last()
            .KodeTransaksi
            .Should()
            .EndWith("001");
    }
    public void Dispose()
    {
        _webApp.Cleanup();
    }
}