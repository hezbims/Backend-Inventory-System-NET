using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Inventory_Backend.Tests.TestConfiguration.Mock;
using Inventory_Backend.Tests.TestConfiguration.Seeder;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend.Tests.PostPengajuanTest.KodeBarangTest;

// [Collection(TestConstant.UnitTestWithDbCollection)]
// public class KodeBarangPengeluaranDuaKaliSehariTest : IDisposable
// {
//     private readonly MyDbFixture _fixture;
//     private readonly SqliteCache _cache;
//     private readonly User _nonAdmin;
//     private readonly Pengaju _grup;
//     private readonly Barang[] _barangs;
//
//     private readonly DateTimeOffset _mockDay = new DateTimeOffset(
//         year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0,
//         offset: TimeSpan.Zero
//     );
//     private readonly TimeZoneInfo _mockTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
//
//     public KodeBarangPengeluaranDuaKaliSehariTest(MyDbFixture fixture)
//     {
//         _fixture = fixture;
//         _cache = new SqliteCache(new SqliteCacheOptions());
//         _cache.Clear();
//
//         using var db = _fixture.CreateContext();
//         db.SeedThreeUser();
//         db.SeedDuaPengaju();
//         db.SeedBarang20WithQuantity20();
//         _nonAdmin = db.Users.First(user => !user.IsAdmin);
//         _grup = db.Pengajus.First(pengaju => !pengaju.IsPemasok);
//
//         _barangs = db.Barangs.ToArray();
//     }
//
//     [Fact]
//     public void Test_Kode_Urutan_Pengajuan_Pengeluaran_Dua_Kali_Dalam_Sehari()
//     {
//         using var db = _fixture.CreateContext();
//
//         var mockTimeProvider = new Mock<TimeProvider>();
//         mockTimeProvider.Setup(provider => provider.GetUtcNow()).Returns(_mockDay);
//         mockTimeProvider.Setup(provider => provider.LocalTimeZone).Returns(_mockTimeZone);
//         
//         var controller = new PostPengajuanController(
//             db: db,
//             cache: _cache,
//             httpContextAccessor: new MockHttpContextAccessor(_nonAdmin),
//             timeProvider: mockTimeProvider.Object
//         );
//
//         var actionResult = controller.Index(new SubmitPengajuanBody
//         {
//             IdPengajuan = null,
//             IdPengaju = _grup.Id,
//             BarangAjuans = new List<BarangAjuanBody>
//             {
//                 new BarangAjuanBody
//                 {
//                     IdBarang = _barangs[0].Id,
//                     Keterangan = null,
//                     Quantity = 1
//                 }
//             }
//         });
//         Assert.IsType<OkObjectResult>(actionResult);
//
//         var actionResult2 = controller.Index(new SubmitPengajuanBody
//         {
//             IdPengaju = _grup.Id,
//             BarangAjuans = new List<BarangAjuanBody>
//             {
//                 new BarangAjuanBody
//                 {
//                     IdBarang = _barangs[0].Id,
//                     Keterangan = null,
//                     Quantity = 1
//                 }
//             }
//         });
//         Assert.IsType<OkObjectResult>(actionResult2);
//
//         var x = db.Pengajuans.ToArray();
//
//         Assert.Equal(2 , db.Pengajuans.Where(
//             pengajuan => 
//                 pengajuan.KodeTransaksi == "TRX-OUT-2000-01-01-001" ||
//                 pengajuan.KodeTransaksi == "TRX-OUT-2000-01-01-002"
//         ).Count());
//     }
//
//     public void Dispose()
//     {
//         _fixture.Cleanup();
//     }
// }