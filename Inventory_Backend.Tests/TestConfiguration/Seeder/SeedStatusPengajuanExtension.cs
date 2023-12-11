using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend.Tests.TestConfiguration.Seeder;

public static class SeedStatusPengajuanExtension
{
    public static void SeedStatusPengajuan(
        this MyDbContext db    
    )
    {
        db.RemoveRange(db.StatusPengajuans);
        db.AddRange(
            StatusPengajuan.Diterima,
            StatusPengajuan.Menunggu,
            StatusPengajuan.Ditolak
        );
        db.SaveChanges();
    }
}