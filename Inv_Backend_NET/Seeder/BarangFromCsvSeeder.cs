using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Seeder.Data;

namespace Inventory_Backend_NET.Seeder;

public class BarangFromCsvSeeder
{
    private readonly MyDbContext _db;
    public BarangFromCsvSeeder(MyDbContext db)
    {
        _db = db;
    }
    
    public List<Barang> Run(
        List<Kategori> listKategori,
        Random rand
    )
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = arg => arg.Header.ToUpper(),
        };
        using var reader = new StreamReader("Seeder/Data/barang_seeder.csv");
        using var csv = new CsvReader(reader, config);
        var listBarang = csv
            .GetRecords<BarangCsvSeederDto>()
            .ToList()
            .Select(
                barangCsvDto =>
                {
                    var rak = barangCsvDto.Rak;
                    var barang = new Barang(
                        nama: barangCsvDto.ItemDescription,
                        kodeBarang: $"R{rak.NomorRak}-{rak.NomorLaci}-{rak.NomorKolom}",
                        kategoriId: listKategori[rand.Next() % listKategori.Count].Id,
                        minStock: barangCsvDto.MinStock,
                        nomorRak: rak.NomorRak,
                        nomorLaci: rak.NomorLaci,
                        nomorKolom: rak.NomorKolom,
                        currentStock: barangCsvDto.Actual ?? 0,
                        lastMonthStock: barangCsvDto.LastMonthStock ?? 0,
                        unitPrice: barangCsvDto.IntUnitPrice,
                        uom: barangCsvDto.Uom
                    );
                    _db.Barangs.Add(barang);
                    return barang;
                }
            )
            .ToList();
        _db.SaveChanges();

        return listBarang;
    }
}