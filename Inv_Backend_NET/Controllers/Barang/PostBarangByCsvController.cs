using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Service;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Barang;


[Route("api/barang/submit-csv")]
public class PostBarangByCsvController : ControllerBase
{
    private readonly MyDbContext _db;

    public PostBarangByCsvController(MyDbContext db)
    {
        _db = db;
    }

    [HttpPost]
    public IActionResult Index(
        [FromForm] CsvUploadModel uploadModel    
    )
    {
        using var transaction = _db.Database.BeginTransaction();
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { message = "File CSV tidak ditemukan" });
            }

            using (var streamReader = new StreamReader(uploadModel.Csv!.OpenReadStream()))
            using (var csvReader = MyCsvReader.From(streamReader))
            {
                var results = csvReader
                    .GetRecords<CsvBarangDto>()!
                    .ToList();
                foreach (var barangRow in results)
                {
                    var currentKategori = _db.Kategoris.FirstOrDefault(
                        kategori => kategori.Nama == barangRow.Kategori
                    );
                    if (currentKategori == null)
                    {
                        currentKategori = new Models.Kategori(nama: barangRow.Kategori);
                        _db.Add(currentKategori);
                    }
                    
                    
                    _db.Barangs.Add(new Models.Barang(
                        nama: barangRow.NamaBarang,
                        kategori: currentKategori,
                        minStock: barangRow.MinStock,
                        nomorRak: barangRow.NomorRak,
                        nomorLaci: barangRow.NomorLaci,
                        nomorKolom: barangRow.NomorKolom,
                        currentStock: barangRow.CurrentStock,
                        lastMonthStock: barangRow.LastMonthStock,
                        unitPrice: barangRow.UnitPrice,
                        uom: barangRow.Uom
                    ));
                    _db.SaveChanges();
                }
                
                transaction.Commit();
                return Ok(new { message = "Sukses" });
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            transaction.Rollback();
            return StatusCode(500, new
            {
                message = e.Message
            });
        }
    }

    public class CsvUploadModel
    {
        [JsonPropertyName("csv")]
        [Required]
        public IFormFile? Csv { get; init; }
    }

    public class CsvBarangDto
    {
        [Name("KODE BARANG")] 
        public string KodeBarang { get; init; } = null!;
        
        [Name("NAMA BARANG")]
        public string NamaBarang { get; init; } = null!;
        
        [Name("KATEGORI")]
        public string Kategori { get; init; } = null!;
        
        [Name("NOMOR RAK")]
        public int NomorRak { get; init; }
        
        [Name("NOMOR LACI")]
        public int NomorLaci { get; init; }
        
        [Name("NOMOR KOLOM")]
        public int NomorKolom { get; init; }
        
        [Name("CURRENT STOCK")]
        public int CurrentStock { get; init; }
        
        [Name("MIN. STOCK")]
        public int MinStock { get; init; }
        
        [Name("LAST MONTH STOCK")]
        public int LastMonthStock { get; init; }
        
        [Name("UNIT PRICE")]
        public int UnitPrice { get; init; }
        
        [Name("UOM")]
        public string Uom { get; init; }
    }
}