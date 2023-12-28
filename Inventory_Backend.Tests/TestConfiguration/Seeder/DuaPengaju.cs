using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.TestConfiguration.Seeder;

public static class DuaPengajuExtension
{

    public static (Pengaju , Pengaju) SeedDuaPengaju(this MyDbContext db)
    {
        var grup = new Pengaju(nama: "grup", isPemasok: false);
        var pemasok = new Pengaju(nama: "pemasok", isPemasok: true);
        db.Pengajus.AddRange(grup , pemasok);
        db.SaveChanges();
        
        
        return (
            db.Pengajus.Single(pengaju => pengaju.IsPemasok), 
            db.Pengajus.Single(pengaju => !pengaju.IsPemasok)
        );
    }
}