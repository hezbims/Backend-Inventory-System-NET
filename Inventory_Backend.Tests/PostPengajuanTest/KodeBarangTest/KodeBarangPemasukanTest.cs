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
// public class KodeBarangPemasukanTest : IDisposable
// {
//
//     private readonly MyDbFixture _fixture;
//     private readonly SqliteCache _cache;
//     private readonly User _admin;
//     private readonly Pengaju _pemasok;
//     private readonly Barang[] _barangs;
//
//     private readonly DateTimeOffset _mockDateTime = new DateTimeOffset(
//         year: 2000, month: 1, day: 1, hour: 0, minute: 0, second: 0,
//         offset: TimeSpan.Zero
//     );
//     private readonly TimeZoneInfo _mockTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Jakarta");
//     
//     public KodeBarangPemasukanTest(MyDbFixture fixture)
//     {
//         _fixture = fixture;
//         _cache = new SqliteCache(new SqliteCacheOptions());
//         _cache.Clear();
//
//         using var db = _fixture.CreateContext();
//         db.SeedThreeUser();
//         db.SeedDuaPengaju();
//         db.SeedBarang20WithQuantity20();
//         _admin = db.Users.First(user => user.IsAdmin);
//         _pemasok = db.Pengajus.First(pengaju => pengaju.IsPemasok);
//
//         _barangs = db.Barangs.ToArray();
//     }
//
//     [Fact]
//     public void Test_Ketika_Pengajuannya_Pemasukan_Maka_Kodenya_In()
//     {
//         using var db = _fixture.CreateContext();
//
//         var mockTimeProvider = new Mock<TimeProvider>();
//         mockTimeProvider.Setup(provider => provider.GetUtcNow()).Returns(_mockDateTime);
//         mockTimeProvider.Setup(provider => provider.LocalTimeZone).Returns(_mockTimeZone);
//         
//         var controller = new PostPengajuanController(
//             db: db,
//             httpContextAccessor: new MockHttpContextAccessor(_admin),
//             cache: _cache,
//             timeProvider: mockTimeProvider.Object
//         );
//
//         var actionResult = controller.Index(new SubmitPengajuanBody
//         {
//             IdPengaju = _pemasok.Id,
//             BarangAjuans = new List<BarangAjuanBody>
//             {
//                 new BarangAjuanBody
//                 {
//                     IdBarang = _barangs.First().Id,
//                     Quantity = 1
//                 }
//             }
//         });
//         Assert.IsType<OkObjectResult>(actionResult);
//
//         Assert.Single(db.Pengajuans.Where(pengajuan => 
//             pengajuan.KodeTransaksi == "TRX-IN-2000-01-01-001"
//         ));
//     }
//
//     public void Dispose()
//     {
//         _fixture.Cleanup();
//     }
// }