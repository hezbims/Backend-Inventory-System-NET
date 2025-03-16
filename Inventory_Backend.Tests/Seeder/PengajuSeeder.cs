using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.Seeder;

public class PengajuSeeder(
    MyDbContext dbContext) : IMySeeder
{
    public Pengaju Run(
        bool isPemasok,
        string name)
    {
        ;
        Pengaju pengaju = new Pengaju(
            nama: name,
            isPemasok: isPemasok);
        dbContext.Add(pengaju);
        dbContext.SaveChanges();
        
        return pengaju;
    }
    
}