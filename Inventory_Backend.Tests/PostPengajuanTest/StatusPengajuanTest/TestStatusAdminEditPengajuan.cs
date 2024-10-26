using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.StatusPengajuanTest;

/// <summary>
/// Memastikan nilai status pengajuan sesuai ekspetasi ketika admin meng-edit suatu pengajuan
/// </summary>
[Collection(TestConstant.IntegrationTestDefinition)]
public class TestStatusAdminEditPengajuan : IDisposable
{
    private readonly CompleteTestData _testData;
    private readonly TestWebAppFactory _webApp;

    public TestStatusAdminEditPengajuan(
        TestWebAppFactory webApp,
        ITestOutputHelper logger)
    {
        _webApp = webApp;
        _webApp.ConfigureLoggingToTestOutput(logger);

        using var db = _webApp.GetDbContext();
        _testData = new CompleteTestSeeder(db: db).Run();
    }

    [Theory]
    [InlineData(StatusPengajuan.DiterimaValue)]
    [InlineData(StatusPengajuan.DitolakValue)]
    public async Task Admin_Dapat_Mengubah_Status_Pengajuan_Dari_Menunggu_Menjadi_Diterima_dan_Menolak(
        string expectedStatusPengajuan)
    {
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);

        var previousPengajuan = _testData.ListPengajuan[2];
        var response = await adminClient.PostAsJsonAsync(
            TestConstant.ApiEndpoints.PostPengajuan,
            new PostPengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.PengajuId,
                ListBarangAjuan =
                [
                    new BarangAjuanRequest
                    {
                        Quantity = 1,
                        IdBarang = previousPengajuan.BarangAjuans.First().BarangId
                    }
                ],
                StatusPengajuanString = expectedStatusPengajuan
            });
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        
        await using var db = _webApp.GetDbContext();
        
        db.Pengajuans.Should().ContainSingle(pengajuan => 
            pengajuan.Id == previousPengajuan.Id &&
            pengajuan.Status.Value == expectedStatusPengajuan);
    }

    [Fact]
    public async Task Test_Admin_Tidak_Dapat_Mengedit_Pengajuan_Dengan_Status_Menunggu_Ke_Status_Menunggu()
    {
        
    }

    [Theory]
    [InlineData(StatusPengajuan.DiterimaValue, StatusPengajuan.DitolakValue)]
    [InlineData(StatusPengajuan.DitolakValue, StatusPengajuan.DiterimaValue)]
    [InlineData(StatusPengajuan.DitolakValue, StatusPengajuan.MenungguValue)]
    [InlineData(StatusPengajuan.DiterimaValue, StatusPengajuan.MenungguValue)]
    public async Task Test_Admin_Tidak_Dapat_Mengubah_Pengajuan_Yang_Statusnya_Sudah_Diterima_Atau_Ditolak(
        string previousStatus, string nextStatus)
    {
        var adminClient = _webApp.GetAuthorizedClient(isAdmin: true);

        var previousPengajuan = _testData.ListPengajuan.First(
            pengajuan => pengajuan.Status.Value == previousStatus &&
                         !pengajuan.Pengaju.IsPemasok);

        var response = await adminClient.PostAsJsonAsync(
            TestConstant.ApiEndpoints.PostPengajuan,
            new PostPengajuanRequest
            {
                IdPengajuan = previousPengajuan.Id,
                IdPegaju = previousPengajuan.PengajuId,
                ListBarangAjuan =
                [
                    new BarangAjuanRequest
                    {
                        Quantity = 1,
                        IdBarang = previousPengajuan.BarangAjuans.First().BarangId
                    }
                ],
                StatusPengajuanString = nextStatus
            });
        response.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}