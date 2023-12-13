using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Service;
using Inventory_Backend_NET.UseCases;
using Inventory_Backend_NET.UseCases.Barang;
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
                var errors = ModelState.First().Value!.Errors.Select(e => e.ErrorMessage).ToArray();
                return BadRequest(new ErrorModel(errors));
            }

            using (var streamReader = new StreamReader(uploadModel.Csv!.OpenReadStream()))
            using (var csvReader = MyCsvReader.From(streamReader))
            {
                var headerError = csvReader.ValidateHeader();
                if (headerError != null)
                {
                    return BadRequest(new ErrorModel(headerError));
                }
                
                var rows = csvReader
                    .GetRecords<CsvBarangDto>()!
                    .ToList();
                
                
                foreach (var barangRow in rows)
                {
                    var currentKategori = _db.Kategoris.FirstOrDefault(
                        kategori => kategori.Nama == barangRow.NamaKategori
                    );
                    if (currentKategori == null)
                    {
                        currentKategori = new Models.Kategori(nama: barangRow.NamaKategori!);
                        _db.Add(currentKategori);
                    }
                    
                    
                    _db.Barangs.Add(new Models.Barang(
                        nama: barangRow.NamaBarang!,
                        kategori: currentKategori,
                        minStock: barangRow.MinStock ?? default,
                        nomorRak: barangRow.NomorRak ?? default,
                        nomorLaci: barangRow.NomorLaci ?? default,
                        nomorKolom: barangRow.NomorKolom ?? default,
                        currentStock: barangRow.CurrentStock ?? default,
                        lastMonthStock: barangRow.LastMonthStock ?? default,
                        unitPrice: barangRow.UnitPrice ?? default,
                        uom: barangRow.Uom!
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
            return StatusCode(500, new ErrorModel(e.Message));
        }
    }
    

    public class CsvUploadModel
    {
        [JsonPropertyName("csv")]
        [Required(ErrorMessage = "Tolong pilih file anda")]
        [AllowedFileExtensions(new []{ ".csv" })]
        public IFormFile? Csv { get; init; }
    }

    public class CsvBarangDto
    {
        [Name("KODE BARANG")] 
        public string? KodeBarang { get; init; }
        
        [Name("NAMA BARANG")]
        public string? NamaBarang { get; init; }
        
        [Name("KATEGORI")]
        public string? NamaKategori { get; init; }
        
        [Name("NOMOR RAK")]
        public int? NomorRak { get; init; }
        
        [Name("NOMOR LACI")]
        public int? NomorLaci { get; init; }
        
        [Name("NOMOR KOLOM")]
        public int? NomorKolom { get; init; }
        
        [Name("CURRENT STOCK")]
        public int? CurrentStock { get; init; }
        
        [Name("MIN. STOCK")]
        public int? MinStock { get; init; }
        
        [Name("LAST MONTH STOCK")]
        public int? LastMonthStock { get; init; }
        
        [Name("UNIT PRICE")]
        public int? UnitPrice { get; init; }
        
        [Name("UOM")]
        public string? Uom { get; init; }
    }

    public class ErrorModel
    {
        [JsonPropertyName("errors")]
        public string[] Errors { get; init; }

        public ErrorModel(string error)
        {
            Errors = new[] { error };
        }

        public ErrorModel(string[] errors)
        {
            Errors = errors;
        }
    }
}