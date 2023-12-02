using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Seeder.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<MyDbContext>(
    options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("SQLServerConnection"))
);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthentication().AddBearerToken();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var db = services.GetRequiredService<MyDbContext>();
    
    if (args.Contains("refresh"))
    {
        var allTables = db.Model.GetEntityTypes().Select(e => e.GetTableName()).ToList();
        foreach (var table in allTables)
        {
            db.Database.ExecuteSqlRaw($"DELETE FROM {table}");
        }
    }
    
    if (args.Contains("test-seeder"))
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = args => args.Header.ToUpper(),
        };
        
        using (var reader = new StreamReader("Seeder/Data/barang_seeder.csv"))
        using (var csv = new CsvReader(reader, config))
        {
            var rand = new Random(5);
            var barangs = csv.GetRecords<BarangCsvDto>().ToList();
            for (var i = 1; i <= 10; ++i)
            {
                db.Kategoris.Add(new Kategori { Nama = $"Kategori {i}" });
            }
            db.SaveChanges();
            
            foreach (var barang in barangs)
            {
                var rak = barang.Rak;
                var namaKategori = $"Kategori {rand.Next() % 10 + 1}";
                db.Barangs.Add(new Barang
                {
                    KodeBarang = $"R{rak.NomorRak}-{rak.NomorLaci}-{rak.NomorKolom}",
                    Nama = barang.ItemDescription,
                    Kategori = db.Kategoris.Where(kategori => 
                        kategori.Nama == namaKategori
                    ).First(),
                    MinStock = barang.MinStock,
                    NomorRak = rak.NomorRak,
                    NomorLaci = rak.NomorLaci,
                    NomorKolom = rak.NomorKolom,
                    CurrentStock = barang.Actual ?? 0,
                    LastMonthStock = barang.LastMonthStock ?? 0,
                    UnitPrice = barang.IntUnitPrice,
                    Uom = barang.Uom
                });    
            }
            db.SaveChanges();
        }
    } 
}

if (args.Length > 0) { return; }

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();