using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend.Tests.TestData;

/// <summary>
/// <ul> Memasukkan barang, pengaju, user, dan pengajuan ke dalam database
/// <li>Pengajuan pertama statusnya diterima</li>
/// <li>Pengajuan kedua statusnya ditolak</li>
/// <li>Pengajuan ketiga statusnya menunggu</li>
/// </ul>
/// </summary>
public class CompleteTestSeeder : IDisposable
{
    private readonly BasicTestSeeder _basicTestSeeder;
    private readonly MyDbContext _db;

    public CompleteTestSeeder(MyDbContext db)
    {
        _basicTestSeeder = new BasicTestSeeder(db);
        _db = db;
    }

    public CompleteTestData Run()
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
        
        var getKodeTransaksi = new GetKodeTransaksiPengajuanUseCase(db: _db);
        var createdAt = TestTimeProvider.Instance.GetUtcNow();
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

        return CompleteTestData.CreateFrom(db: _db);
    }

    public void Dispose()
    {
        _db.Dispose();
    }
}

public record CompleteTestData
{
    public required User Admin { get; init; }
    public required User NonAdmin { get; init; }
    public required List<Barang> ListBarang { get; init; }
    public required Pengaju Grup { get; init; }
    public required Pengaju Pemasok { get; init; }
    public required List<Pengajuan> ListPengajuan { get; init; }

    public static CompleteTestData CreateFrom(MyDbContext db)
    {
        return new CompleteTestData
        {
            Admin = db.Users.First(user => user.IsAdmin),
            NonAdmin = db.Users.First(user => !user.IsAdmin),
            ListBarang = db.Barangs.ToList(),
            Grup = db.Pengajus.First(pengaju => !pengaju.IsPemasok),
            Pemasok = db.Pengajus.First(pengaju => pengaju.IsPemasok),
            ListPengajuan = db.Pengajuans
                .Include(pengajuan => pengajuan.BarangAjuans)
                .Include(pengajuan => pengajuan.User)
                .Include(pengajuan => pengajuan.Pengaju)
                .ToList()
            
        };
    }
} 