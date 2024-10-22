using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class MengajukanTigaKaliTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly BasicTestData _testData;

    public MengajukanTigaKaliTest(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);

        using var db = _webApp.GetDbContext();
        _testData = new BasicTestSeeder(db: db).Run();
    }

    [Fact]
    public async Task Test_Ketika_Mengajukan_Sukses_Tiga_Kali_Maka_Setiap_Transaksi_Kode_Transaksinya_Benar()
    {
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);

        for (var i = 0; i < 2; i++)
        {
            var response = await adminClient.PostAsJsonAsync("/api/pengajuan/add", new PostPengajuanRequest
            {
                IdPegaju = _testData.Pemasok.Id,
                ListBarangAjuan = [
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang.First().Id,
                        Quantity = 1
                    }
                ],
                StatusPengajuanString = StatusPengajuan.DiterimaValue
            });
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }
        
        var response2 = await nonAdminClient.PostAsJsonAsync("/api/pengajuan/add", new PostPengajuanRequest
        {
            IdPegaju = _testData.Grup.Id,
            ListBarangAjuan = [
            new BarangAjuanRequest
            {
                IdBarang = _testData.ListBarang.First().Id,
                Quantity = 1
            }]
        });
        response2.StatusCode.Should().Be(HttpStatusCode.OK);

        List<string> expectedKodeTransaksi =
        [
            "TRX-IN-2024-10-01-001",
            "TRX-IN-2024-10-01-002",
            "TRX-OUT-2024-10-01-003",
        ];

        await using var db = _webApp.GetDbContext();
        var listPengajuan = db.Pengajuans.ToList();

        for (var i = 0; i < listPengajuan.Count(); i++)
        {
            expectedKodeTransaksi.Should().Contain(listPengajuan[i].KodeTransaksi);
        }
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}