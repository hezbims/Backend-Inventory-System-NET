using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Pengajuan._Logic;
using Inventory_Backend.Tests.TestData;

namespace Inventory_Backend.Tests.Seeder;

public class PengajuanSeeder(
    MyDbContext dbContext,
    TimeProvider timeProvider,
    GetKodeTransaksiPengajuanUseCase getKodeTransaksiPengajuanUseCase
) : IMySeeder
{
    public List<Pengajuan> Run(
        Pengaju pengaju,
        User user,
        List<BarangAjuan> barangAjuans,
        string? kodeTransaksi = null,
        StatusPengajuan? status = null,
        long? createdAt = null,
        int totalItem = 1
    )
    {
        createdAt ??= timeProvider.GetUtcNow().ToUnixTimeMilliseconds();
        kodeTransaksi ??= getKodeTransaksiPengajuanUseCase.Run(
            DateTimeOffset.FromUnixTimeMilliseconds(createdAt.Value),
            pengaju);
        status ??= StatusPengajuan.Diterima;
        
        List<Pengajuan> pengajuans = new List<Pengajuan>();
        for (int i = 0; i < totalItem; i++)
            pengajuans.Add(new Pengajuan(
                status: status,
                kodeTransaksi: kodeTransaksi,
                pengaju: pengaju,
                user: user,
                createdAt: createdAt.Value,
                updatedAt: createdAt.Value,
                barangAjuans: barangAjuans));
        dbContext.Pengajuans.AddRange(pengajuans);
        dbContext.SaveChanges();

        return pengajuans;
    }
}