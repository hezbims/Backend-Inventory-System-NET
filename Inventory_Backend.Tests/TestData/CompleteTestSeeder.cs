using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;

namespace Inventory_Backend.Tests.TestData;

public class CompleteTestSeeder : IDisposable
{
    private readonly BasicTestSeeder _basicTestSeeder;
    private readonly MyDbContext _db;

    public CompleteTestSeeder(MyDbContext db)
    {
        _basicTestSeeder = new BasicTestSeeder(db);
        _db = db;
    }

    public void Run()
    {
        _basicTestSeeder.Run();
        var listBarang = _db.Barangs.ToList();
        var group = _db.Pengajus.First(pengaju => !pengaju.IsPemasok);
        var nonAdmin = _db.Users.First(user => !user.IsAdmin);
        var statusPengajuanEntries = new List<StatusPengajuan>
        {
            StatusPengajuan.Diterima,
            StatusPengajuan.Ditolak,
            StatusPengajuan.Menunggu
        };

        var timeProvider = new TestTimeProvider();
        var getKodeTransaksi = new GetKodeTransaksiPengajuanUseCase(db: _db);
        var createdAt = timeProvider.GetUtcNow();
        foreach (var statusPengajuan in statusPengajuanEntries)
        {
            _db.Pengajuans.Add(
                new Pengajuan(
                    pengaju: group,
                    status: statusPengajuan,
                    user: nonAdmin,
                    createdAt: createdAt.ToUnixTimeMilliseconds(),
                    kodeTransaksi: getKodeTransaksi.Run(
                        dateCreatedAt: createdAt, pengaju: group),
                    barangAjuans: new List<BarangAjuan>
                    {
                        new BarangAjuan(
                            barang: listBarang.First(),
                            quantity: 1,
                            keterangan: null)
                    }
                ));
            _db.SaveChanges();
        }
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}