using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

/// <summary>
/// Memastikan kode tipe transaksi pada pengajuan benar setelah pengajuan baru dibuat. <br></br>
/// Contoh : "TRX-IN-2020-09-01", maka "IN" adalah kode tipe transaksi
/// </summary>
[Collection(TestConstant.IntegrationTestDefinition)]
public class KodeTipeTransaksiFromCreatePengajuanTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly BasicTestData _testData;
    public KodeTipeTransaksiFromCreatePengajuanTest(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);
        
        using var db = _webApp.GetDbContext();
        _testData = new BasicTestSeeder(db: db).Run();
    }

    [Fact]
    public async Task Test_Ketika_Pengajuannya_Pemasukan_Maka_Kodenya_In()
    {
        await using var db = _webApp.GetDbContext();
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);
        
        var response = await adminClient.PostAsJsonAsync(
            "/api/pengajuan/add",
            new CreatePengajuanRequest
            {
                IdPegaju = _testData.Pemasok.Id,
                ListBarangAjuan = [
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang.First().Id,
                        Quantity = 1,
                        Keterangan = ""
                    }
                ]
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        db.Pengajuans
            .Should()
            .ContainSingle()
            .And
            .Satisfy(pengajuan =>
                pengajuan.KodeTransaksi == "TRX-IN-2024-10-01-001");
    }

    [Fact]
    public async Task Test_Ketika_Pengajuannya_Pengeluaran_Maka_Kodenya_Out()
    {
        await using var db = _webApp.GetDbContext();
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);
        
        var response = await nonAdminClient.PostAsJsonAsync(
            "/api/pengajuan/add",
            new CreatePengajuanRequest
            {
                IdPegaju = _testData.Grup.Id,
                ListBarangAjuan = [
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang.First().Id,
                        Quantity = 1,
                        Keterangan = ""
                    }
                ]
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        db.Pengajuans
            .Should()
            .ContainSingle()
            .And
            .Satisfy(pengajuan =>
                pengajuan.KodeTransaksi == "TRX-OUT-2024-10-01-001");
    }
    
    public void Dispose()
    {
        _webApp.Cleanup();
    }
}