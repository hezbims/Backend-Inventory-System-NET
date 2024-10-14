using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.Helper;
using Inventory_Backend.Tests.PostPengajuanTest.Model;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestData;
using Microsoft.Extensions.DependencyInjection;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.PostPengajuanTest.StatusPengajuanTest;

[Collection(TestConstant.IntegrationTestDefinition)]
public class TestStatusBuatPengajuanBaru : IDisposable
{
    private readonly HttpClient _client;
    private readonly TestWebAppFactory _webApp;
    private readonly Pengaju _pemasok;
    private readonly Pengaju _grup;
    private readonly Barang _barang;
    private readonly string _adminToken;
    private readonly ITestOutputHelper _logger;
    
    public TestStatusBuatPengajuanBaru(
        TestWebAppFactory webApp,
        ITestOutputHelper logger 
    )
    {
        _logger = logger;
        _webApp = webApp;
        _webApp.Logger = logger;
        _client = webApp.CreateClient();

        using (var scope = _webApp.Services.CreateScope())
        {
            var db = scope.ServiceProvider.GetService<MyDbContext>()!;

            new BasicTestSeeder(db).Run();
            _pemasok = db.Pengajus.First(pengaju => pengaju.IsPemasok);
            _grup = db.Pengajus.First(pengaju => !pengaju.IsPemasok);
            _barang = db.Barangs.First();
            _adminToken = scope.GetJwt(isAdmin: true);
        }
    }

    [Fact]
    public async Task Test_Kalau_Admin_Yang_Buat_Pengajuan_Maka_Statusnya_Diterima()
    {
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer" , _adminToken);
        
        var response = await _client.PostAsJsonAsync(
            "/api/pengajuan/add", 
            new CreatePengajuanRequest
            {
                IdPegaju = _grup.Id,
                ListBarangAjuan = new List<BarangAjuanRequest>
                {
                    new BarangAjuanRequest
                    {
                        IdBarang = _barang.Id,
                        Quantity = 1,
                        Keterangan = ""
                    }
                }
            });
        
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        await using var db = _webApp.GetDbContext();
        db.Pengajuans
            .Should()
            .ContainSingle()
            .And
            .Satisfy(pengajuan => pengajuan.Status == StatusPengajuan.Diterima);
    }

    // [Fact]
    // public void Test_Kalau_Admin_Yang_Buat_Pengajuan_Baru_Maka_Statusnya_Diterima()
    // {
    //     using var db = _fixture.CreateContext();
    //
    //     var adminContext = new MockHttpContextAccessor(_admin);
    //     var controller = new PostPengajuanController(
    //         db: db, 
    //         httpContextAccessor: adminContext, 
    //         cache: _cache,
    //         timeProvider: TimeProvider.System
    //     );
    //
    //     var actionResult = controller.Index(new SubmitPengajuanBody
    //     {
    //         BarangAjuans = new List<BarangAjuanBody>()
    //         {
    //             new BarangAjuanBody
    //             {
    //                 IdBarang = _barangs.First().Id,
    //                 Keterangan = null,
    //                 Quantity = 1
    //             }
    //         },
    //         IdPengaju = _grup.Id
    //     });
    //
    //     Assert.IsType<OkObjectResult>(actionResult);
    //
    //     var pengajuan = db.Pengajuans.Single();
    //     Assert.Equal(StatusPengajuan.DiterimaValue , pengajuan.Status.Value);
    // }

    // [Fact]
    // public void Test_Kalau_Non_Admin_Buat_Pengajuan_Baru_Maka_Statusnya_Menunggu()
    // {
    //     using var db = _fixture.CreateContext();
    //
    //     var nonAdminContext = new MockHttpContextAccessor(_nonAdmin);
    //     var controller = new PostPengajuanController(
    //         db: db, 
    //         httpContextAccessor: nonAdminContext, 
    //         cache: _cache,
    //         timeProvider: TimeProvider.System
    //     );
    //     
    //     var actionResult = controller.Index(new SubmitPengajuanBody
    //     {
    //         BarangAjuans = new List<BarangAjuanBody>
    //         {
    //             new BarangAjuanBody
    //             {
    //                 IdBarang = _barangs.First().Id,
    //                 Keterangan = null,
    //                 Quantity = 1
    //             }
    //         },
    //         IdPengaju = _grup.Id
    //     });
    //
    //     Assert.IsType<OkObjectResult>(actionResult);
    //     var pengajuan = db.Pengajuans.Single();
    //     Assert.Equal(StatusPengajuan.MenungguValue , pengajuan.Status.Value);
    // }

    public void Dispose()
    {
        _webApp.Cleanup();
    }
}