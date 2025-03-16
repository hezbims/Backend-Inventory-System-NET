using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.Seeder;

public class KategoriSeeder(
    MyDbContext dbContext) : IMySeeder
{
    public List<Kategori> Run(int totalItems)
    {
        List<Kategori> kategoris = [];
        for (int i = 1; i <= totalItems; i++)
            kategoris.Add(new Kategori(
                nama: $"kategori-{i}"));
        dbContext.Kategoris.AddRange(kategoris);
        dbContext.SaveChanges();
        return kategoris;
    }
}