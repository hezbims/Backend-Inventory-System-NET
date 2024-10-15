using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Logging;
using Inventory_Backend.Tests.TestData;
using Microsoft.EntityFrameworkCore;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class EditPengajuanKodeTransaksiUnchangedTest : IDisposable
{
    private readonly TestWebAppFactory _webApp;
    public EditPengajuanKodeTransaksiUnchangedTest(
        TestWebAppFactory webApp,
        ITestOutputHelper testOutputHelper)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(testOutputHelper);
        
        using (var db = _webApp.GetDbContext())
        {
            new CompleteTestSeeder(db: db).Run();
        }
    }

    [Fact]
    public async Task Test_Ketika_Edit_Pengajuan_Maka_Kode_Transaksi_Tidak_Akan_Berubah()
    {
        using var db = _webApp.GetDbContext();
        var oldPengajuan = db.Pengajuans
            .Include(pengajuan => pengajuan.User)
            .Include(pengajuan => pengajuan.BarangAjuans)
            .AsSplitQuery()
            .First();
        
        var client = _webApp.GetAuthorizedClient(userId: oldPengajuan.User.Id);
        var response = await client.PostAsJsonAsync(
            "/api/pengajuan/add",
            new CreatePengajuanRequest
            {
                IdPegaju = oldPengajuan.PengajuId,
                IdPengajuan = oldPengajuan.Id,
                ListBarangAjuan = new List<BarangAjuanRequest>
                {
                    new BarangAjuanRequest
                    {
                        IdBarang = oldPengajuan.BarangAjuans.First().BarangId,
                        Quantity = oldPengajuan.BarangAjuans.First().Quantity,
                        Keterangan = "Keterangan",
                    }
                }
            });
        

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        var editedPengajuan = db.Pengajuans.Single(pengajuan => pengajuan.Id == oldPengajuan.Id);
        editedPengajuan.KodeTransaksi.Should().Be(oldPengajuan.KodeTransaksi);
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}