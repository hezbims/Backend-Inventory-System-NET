using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Pengajuan.GetPengajuanPaginated;
using Inventory_Backend_NET.Fitur.Pengajuan.GetPengajuanPaginated._Dto.Response;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.GetPengajuanTest;

// [Collection(TestConstant.UnitTestWithDbCollection)]
// public class PengajuanFilteredByUserTest : IDisposable
// {
//     private readonly MyDbFixture _fixture;
//     private readonly User _admin;
//     private readonly User _nonAdmin;
//     
//     public PengajuanFilteredByUserTest(MyDbFixture fixture)
//     {
//         _fixture = fixture;
//         using var db = _fixture.CreateContext();
//         (_admin, _nonAdmin, _) = db.SeedThreeUser();
//         
//         var barangs = db.SeedBarang20WithQuantity20();
//         var cache = new SqliteCache(options: new SqliteCacheOptions());
//         cache.Clear();
//
//         var timeProvider = TimeProvider.System;
//         db.Pengajuans.Add(new Pengajuan(
//             db: db,
//             pengaju: new Pengaju(nama: "grup-1" , isPemasok: false),
//             status: StatusPengajuan.Diterima,
//             user: _nonAdmin,
//             barangAjuans: new List<BarangAjuan>(new []
//             {
//                 new BarangAjuan(barangId: barangs.First().Id , quantity: 1, keterangan: null)
//             }),
//             timeProvider: timeProvider
//         ));
//         db.Pengajuans.Add(new Pengajuan(
//             db: db,
//             pengaju: new Pengaju(nama: "permasok-1" , isPemasok: true),
//             status: StatusPengajuan.Diterima,
//             user: _admin,
//             barangAjuans: new List<BarangAjuan>(new []
//             {
//                 new BarangAjuan(barangId: barangs.First().Id , quantity: 1, keterangan: null)
//             }),
//             timeProvider: timeProvider
//         ));
//         db.SaveChanges();
//     }
//
//     [Fact]
//     public void Test_Ketika_Admin_Maka_Lihat_Semua_Pengajuan()
//     {
//         using var db = _fixture.CreateContext();
//         var adminContext = new MockHttpContextAccessor(_admin);
//
//         var controller = new GetPengajuanPaginatedController(
//             db: db, 
//             httpContext: adminContext
//         );
//
//         var actionResult = controller.GetPengajuanPaginated(
//             idPengaju: null,
//             keyword: "",
//             page: 1
//         );
//
//         Assert.IsType<OkObjectResult>(actionResult);
//
//         var okResult = (actionResult as OkObjectResult)!;
//         var data = (okResult.Value as PaginatedResult<PengajuanPreviewDto>)!.Data;
//
//         Assert.Equal(2 , data.Count);
//     }
//
//     [Fact]
//     public void Test_Ketika_User_Biasa_Maka_Hanya_Melihat_Pengajuannya_Sendiri()
//     {
//         using var db = _fixture.CreateContext();
//         var nonAdminContext = new MockHttpContextAccessor(_nonAdmin);
//         
//         var controller = new GetPengajuanPaginatedController(
//             db: db, 
//             httpContext: nonAdminContext
//         );
//
//         var actionResult = controller.GetPengajuanPaginated(
//             idPengaju: null,
//             keyword: "",
//             page: 1
//         );
//
//         Assert.IsType<OkObjectResult>(actionResult);
//
//         var okResult = (actionResult as OkObjectResult)!;
//         var data = (okResult.Value as PaginatedResult<PengajuanPreviewDto>)!.Data;
//
//         Assert.Single(data);
//         Assert.Single(data.Where(
//             pengajuan => pengajuan.User.IsAdmin == false    
//         ));
//     }
//
//     public void Dispose()
//     {
//         _fixture.Cleanup();
//     }
// }