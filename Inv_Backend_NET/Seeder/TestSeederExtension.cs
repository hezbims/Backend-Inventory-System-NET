using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Seeder.Data;
using NeoSmart.Caching.Sqlite;

namespace Inventory_Backend_NET.Seeder;

public static class TestSeederExtension
{
    public static void TestSeeder(this IServiceProvider serviceProvider)
    {
        var db = serviceProvider.GetRequiredService<MyDbContext>();
        var sqliteCache = serviceProvider.GetRequiredService<SqliteCache>();
        sqliteCache.Clear();
        
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToUpper(),
        };
        
        using (var reader = new StreamReader("Seeder/Data/barang_seeder.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var rand = new Random(5);

            var kategoris = Enumerable.Range(1, 10).Select(i =>
                {
                    var kategori = new Kategori { Nama = $"Kategori {i}" };
                    db.Kategoris.Add(kategori);
                    return kategori;
                }
            ).ToList();
            
            var barangs = csv
                .GetRecords<BarangCsvDto>()
                .ToList()
                .Select(
                    barangCsvDto =>
                    {
                        var rak = barangCsvDto.Rak;
                        var barang = new Barang
                        {
                            KodeBarang = $"R{rak.NomorRak}-{rak.NomorLaci}-{rak.NomorKolom}",
                            Nama = barangCsvDto.ItemDescription,
                            Kategori = kategoris[rand.Next() % kategoris.Count()],
                            MinStock = barangCsvDto.MinStock,
                            NomorRak = rak.NomorRak,
                            NomorLaci = rak.NomorLaci,
                            NomorKolom = rak.NomorKolom,
                            CurrentStock = barangCsvDto.Actual ?? 0,
                            LastMonthStock = barangCsvDto.LastMonthStock ?? 0,
                            UnitPrice = barangCsvDto.IntUnitPrice,
                            Uom = barangCsvDto.Uom
                        };
                        db.Barangs.Add(barang);
                        return barang;
                    }    
                )
                .ToList();

            var users = new List<User>( new []{
                    new User
                    {
                        Username = "admin",
                        Password = "123",
                        IsAdmin = true,
                    },
                    new User
                    {
                        Username = "hezbi",
                        Password = "123",
                        IsAdmin = false,
                    },
                    new User
                    {
                        Username = "hasbi",
                        Password = "123",
                        IsAdmin = false,
                    }
                }
            );
            foreach (var user in users) { db.Users.Add(user); }

            var pengajus = new List<Pengaju>();
            for (int i = 1 ; i <= 5; i++)
            {
                var grup = new Pengaju
                {
                    Nama = $"Grup {i}",
                    IsPemasok = false
                };
                pengajus.Add(grup);
                db.Pengajus.Add(grup);
            }
            for (int i = 1 ; i <= 3; i++)
            {
                var pemasok = new Pengaju
                {
                    Nama = $"Pemasok {i}",
                    IsPemasok = true
                };
                pengajus.Add(pemasok);
                db.Pengajus.Add(pemasok);
            }

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
                .Select(barang => new BarangAjuan // ubah dari barang ke barang ajuan
                {
                    Barang = barang,
                    Quantity = rand.Next(5) + 1,
                    Keterangan = rand.Next(2) == 0 ? null : "abc"
                })
                .ToList();
            
            db.Pengajuans.Add(
                new Pengajuan(sqliteCache)
                {
                    Pengaju = pengaju,
                    Status = status,
                    User = status.GetRandomUser(users , rand),
                    BarangAjuans = barangAjuans
                }
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