using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest;

/// <summary>
/// Mengecek apakah current stock pada tabel barangs terupdate dengan benar setelah nambah pengajuan baru
/// </summary>
[Collection(TestConstant.IntegrationTestDefinition)]
public class AddNewPengajuanStockQuantityTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly BasicTestData _testData;

    public AddNewPengajuanStockQuantityTest(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);

        using var db = _webApp.GetDbContext();
        _testData = new BasicTestSeeder(db: db).Run();
    }

    [Fact]
    public async Task
        Test_Ketika_Submit_Pengajuan_Baru_Dengan_Pengaju_Tipe_Grup_Berhasil_Maka_Current_Stock_Akan_Berkurang()
    {
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);

        var response = await nonAdminClient.PostAsJsonAsync("/api/pengajuan/add", new CreatePengajuanRequest
        {
            IdPegaju = _testData.Grup.Id,
            ListBarangAjuan =
            [
                new BarangAjuanRequest
                {
                    Quantity = 2,
                    IdBarang = _testData.ListBarang[1].Id
                },
                new BarangAjuanRequest
                {
                    Quantity = 3,
                    IdBarang = _testData.ListBarang[4].Id
                }
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();

        var newListBarang = db.Barangs.ToList();
        List<int> expectedQuantities = [10, 8, 10, 10, 7];

        for (var i = 0; i < expectedQuantities.Count; i++)
        {
            newListBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
        }
    }

    [Fact]
    public async Task 
        Test_Ketika_Submit_Pengajuan_Baru_Dengan_Pengaju_Tipe_Pemasok_Berhasil_Maka_Current_Stock_Akan_Bertambah(){
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);

        var response = await adminClient.PostAsJsonAsync("/api/pengajuan/add", new CreatePengajuanRequest
        {
            IdPegaju = _testData.Pemasok.Id,
            ListBarangAjuan =
            [
                new BarangAjuanRequest
                {
                    Quantity = 1,
                    IdBarang = _testData.ListBarang[2].Id
                },
                new BarangAjuanRequest
                {
                    Quantity = 2,
                    IdBarang = _testData.ListBarang[3].Id
                }
            ]
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();

        var newListBarang = db.Barangs.ToList();
        List<int> expectedQuantities = [10, 10, 11, 12, 10];

        for (var i = 0; i < expectedQuantities.Count; i++)
        {
            newListBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
        }
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}