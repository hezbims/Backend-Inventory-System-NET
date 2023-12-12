using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Seeder.Data;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend_NET.Seeder;

public static class TestSeederExtension
{
    public static void TestSeeder(
        this IServiceProvider serviceProvider,
        string[] args    
    )
    {
        var db = serviceProvider.GetRequiredService<MyDbContext>();
        var sqliteCache = serviceProvider.GetRequiredService<SqliteCache>();
        sqliteCache.Clear();
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = arg => arg.Header.ToUpper(),
        };
        
        using (var reader = new StreamReader("Seeder/Data/barang_seeder.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var rand = new Random(5);

            var kategoris = Enumerable.Range(1, 10).Select(i =>
                {
                    var kategori = new Kategori(nama : $"Kategori {i}");
                    db.Kategoris.Add(kategori);
                    return kategori;
                }
            ).ToList();
            db.SaveChanges();
            
            var barangs = csv
                .GetRecords<BarangCsvSeederDto>()
                .ToList()
                .Select(
                    barangCsvDto =>
                    {
                        var rak = barangCsvDto.Rak;
                        var barang = new Barang(
                            nama : barangCsvDto.ItemDescription,
                            kategoriId : kategoris[rand.Next() % kategoris.Count()].Id,
                            minStock : barangCsvDto.MinStock,
                            nomorRak : rak.NomorRak,
                            nomorLaci : rak.NomorLaci,
                            nomorKolom : rak.NomorKolom,
                            currentStock : barangCsvDto.Actual ?? 0,
                            lastMonthStock : barangCsvDto.LastMonthStock ?? 0,
                            unitPrice : barangCsvDto.IntUnitPrice,
                            uom : barangCsvDto.Uom
                        );
                        db.Barangs.Add(barang);
                        return barang;
                    }    
                )
                .ToList();
            db.SaveChanges();
            

            var users = new List<User>( new []{
                    new User(
                        username : "admin",
                        password : "123",
                        isAdmin : true
                    ),
                    new User(
                        username : "hezbi",
                        password : "123",
                        isAdmin : false
                    ),
                    new User(
                        username : "hasbi",
                        password : "123",
                        isAdmin : false
                    )
                }
            );
            foreach (var user in users) { db.Users.Add(user); }

            var pengajus = new List<Pengaju>();
            for (int i = 1 ; i <= 5; i++)
            {
                var grup = new Pengaju(
                    nama : $"Grup {i}",
                    isPemasok : false
                );
                pengajus.Add(grup);
                db.Pengajus.Add(grup);
            }
            for (int i = 1 ; i <= 3; i++)
            {
                var pemasok = new Pengaju(
                    nama : $"Pemasok {i}",
                    isPemasok : true
                );
                pengajus.Add(pemasok);
                db.Pengajus.Add(pemasok);
            }

            db.SaveChanges();
            
            if (args.Contains("no-pengajuan")) { return; }

            db.SeedPengajuan(
                sqliteCache,
                users,
                barangs,
                pengajus,
                rand
            );

            db.SaveChanges();
        }
    }
}

public static class PengajuanSeederExtension
{
    public static void SeedPengajuan(
        this MyDbContext db,
        SqliteCache sqliteCache,
        List<User> users,
        List<Barang> barangs,
        List<Pengaju> pengajus,
        Random rand
    )
    {
        const int totalPengajuan = 37;
        StatusPengajuan[] listStatus = 
        {
            StatusPengajuan.Diterima,
            StatusPengajuan.Ditolak,
            StatusPengajuan.Menunggu
        };
        
        for (int i = 0; i < totalPengajuan; ++i)
        {
            var pengaju = pengajus[rand.Next(pengajus.Count)];
            var status = listStatus[rand.Next(listStatus.Length)];
            var barangAjuans = Enumerable.Range(0 , barangs.Count)
                .OrderBy(_ => Guid.NewGuid()) // acak urutan indexnya
                .Take(rand.Next(4) + 1) // Ambil 1-5 barang
                .Select(index => barangs[index]) // ubah dari index ke barang
                .Select(barang => new BarangAjuan(
                        barang: barang,
                        quantity: rand.Next(5) + 1,
                        keterangan: rand.Next(2) == 0 ? null : "abc"
                    )
                ) // ubah dari barang ke barang ajuan
                .ToList();
            
            db.Pengajuans.Add(
            new Pengajuan(
                    cache: sqliteCache,
                    pengaju: pengaju,
                    status: status,
                    user: status.GetRandomUser(users, rand),
                    barangAjuans: barangAjuans
                )
            );
        }
    }

    private static User GetRandomUser(
        this StatusPengajuan status,
        List<User> users,
        Random random
    )
    {
        var filteredUsers = users;
        if (status.Value != StatusPengajuan.DiterimaValue)
        {
            filteredUsers = users.Where(user => !user.IsAdmin).ToList();
        }

        return filteredUsers[random.Next(filteredUsers.Count)];
    }
}