using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.StockQuantityTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class EditPengajuanStockQuantityTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    private readonly CompleteTestData _testData;

    public EditPengajuanStockQuantityTest(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);
        
        using var db = _webApp.GetDbContext();
        _testData = new CompleteTestSeeder(db: db).Run();
    }

    /// <summary>
    /// Memastikan stock terupdate dengan benar, kalau admin ngeedit pengajuan dari menunggu ke terima
    /// </summary>
    [Fact]
    public async Task Test_Admin_Terima_Pengajuan_Yang_Sebelumnya_Menunggu_Maka_Stock_Terupdate_Dengan_Benar()
    {
        var adminClient = _webApp.GetAuthorizedClient(userId: _testData.Admin.Id);

        var previousPengajuan = _testData.ListPengajuan[2];
        var response = await adminClient.PostAsJsonAsync(
            "api/pengajuan/add",
            new PostPengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.PengajuId,
                ListBarangAjuan = [
                    new BarangAjuanRequest
                    {
                        Quantity = 2,
                        IdBarang = previousPengajuan.BarangAjuans.Single().BarangId
                    },
                    new BarangAjuanRequest
                    {
                        Quantity = 3,
                        IdBarang = _testData.ListBarang[1].Id
                    }
                ],
                StatusPengajuanString = StatusPengajuan.DiterimaValue
            });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();
        var newBarang = db.Barangs.ToList();
        List<int> expectedQuantities = [8, 7, 10, 10, 10];

        for (int i = 0; i < expectedQuantities.Count; i++)
        {
            newBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
        }
    }

    [Fact]
    public async Task Test_Admin_Edit_Pengajuan_Yang_Sebelumnya_Diterima_Pemasukan_Stock_Terupdate_Dengan_Benar()
    {
        var adminClient = _webApp.GetAuthorizedClient(userId: _testData.Admin.Id);
        var previousPengajuan = _testData.ListPengajuan[3];
        
        var response = await adminClient.PostAsJsonAsync("/api/pengajuan/add", new PostPengajuanRequest
        {
            IdPengajuan = previousPengajuan.Id,
            IdPegaju =  previousPengajuan.PengajuId,
            ListBarangAjuan = [
            new BarangAjuanRequest
            {
                Quantity = 1,
                IdBarang = _testData.ListBarang[0].Id
            }, 
            new BarangAjuanRequest
            {
                Quantity = 2,
                IdBarang = _testData.ListBarang[1].Id
            }],
            StatusPengajuanString = StatusPengajuan.DiterimaValue
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        using var db = _webApp.GetDbContext();
        var newListBarang = db.Barangs.ToList();
        List<int> expectedQuantities = [10, 12, 10, 10, 10];

        for (var i = 0; i < expectedQuantities.Count; i++)
        {
            newListBarang[i].CurrentStock.Should().Be(expectedQuantities[i]);
        }
    }

    [Fact]
    public async Task
        Test_Admin_Mengubah_Pengajuan_Dari_Status_Menunggu_Ke_Menolak_Maka_Tidak_Akan_Ada_Update_Stock()
    {
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);
        var previousPengajuan = _testData.ListPengajuan[2];

        var response = await adminClient.PostAsJsonAsync(
            TestConstant.ApiEndpoints.PostPengajuan,
            new PostPengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.PengajuId,
                ListBarangAjuan = new List<BarangAjuanRequest>
                {
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang[1].Id,
                        Quantity = 100
                    }
                },
                StatusPengajuanString = StatusPengajuan.DitolakValue
            });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();
        db.Barangs.ToList().Should().AllSatisfy(barang => barang.CurrentStock.Should().Be(10));
    }

    [Fact]
    public async Task
        Test_Non_Admin_Edit_Pengajuan_Yang_Masih_Menunggu_Maka_Stock_Akan_Tetap()
    {
        var nonAdminClient = _webApp.GetAuthorizedClient(isAdmin: false);
        var previousPengajuan = _testData.ListPengajuan[2];

        var response = await nonAdminClient.PostAsJsonAsync(
            TestConstant.ApiEndpoints.PostPengajuan,
            new PostPengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.PengajuId,
                ListBarangAjuan = new List<BarangAjuanRequest>
                {
                    new BarangAjuanRequest
                    {
                        IdBarang = _testData.ListBarang[1].Id,
                        Quantity = 100
                    }
                },
            });
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();
        db.Barangs.ToList().Should().AllSatisfy(barang => barang.CurrentStock.Should().Be(10));
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}