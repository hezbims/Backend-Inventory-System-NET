using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.UseCases.Barang;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Barang;

[Route("api/barang/add")]
public class PostFormBarangController : Controller
{
    private readonly MyDbContext _db;
    private readonly ValidateBarangPropertyAvailability _propertyAvailabilityValidator = 
        new ValidateBarangPropertyAvailability();
    
    public PostFormBarangController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpPost]
    public IActionResult Index([FromBody] PostBarangDto requestBody)
    {
        try
        {
            _propertyAvailabilityValidator.Execute(
                id: requestBody.Id,
                db: _db,
                predicate: barang => 
                    barang.NomorRak == requestBody.NomorRak &&
                    barang.NomorLaci == requestBody.NomorLaci &&
                    barang.NomorKolom == requestBody.NomorKolom,
                key : "nomor_rak",
                errorMessage: "Lokasi pada rak, laci, dan kolom ini sudah terpakai",
                modelState: ModelState
            );
            _propertyAvailabilityValidator.Execute(
                id: requestBody.Id,
                db: _db,
                predicate: barang => barang.KodeBarang == requestBody.KodeBarang,
                key: "kode_barang",
                errorMessage: "Kode barang ini sudah terpakai",
                modelState: ModelState
            );

            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value?.Errors.Any() ?? false)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    ); 
                return BadRequest(new { errors });
            }

            var barangModel = requestBody.ToBarang();
            if (requestBody.Id == null)
            {
                _db.Barangs.Add(barangModel);
            }
            else
            {
                _db.Barangs.Update(barangModel);
            }
            _db.SaveChanges();

            return Ok(new
            {
                message = "sukses"
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new
            {
                message = e.Message
            });
        }
    }
}

public class PostBarangDto
{
    [JsonPropertyName(("id"))] 
    public int? Id { get; set; }

    [JsonPropertyName("nama")] 
    [Required] 
    public string Nama { get; set; } = null!;

    [JsonPropertyName("kode_barang")]
    [Required]
    public string KodeBarang { get; set; } = null!;
    
    [JsonPropertyName("nomor_rak")]
    [Required]
    [Range(1, 6)]
    public int? NomorRak { get; set; }
    
    [JsonPropertyName("nomor_laci")]
    [Required]
    [Range(1, 30)]
    public int? NomorLaci { get; set; }
    
    [JsonPropertyName("nomor_kolom")]
    [Required]
    [Range(1, 9)]
    public int? NomorKolom { get; set; }
    
    [JsonPropertyName("stock_sekarang")]
    [Required]
    [Range(1 , int.MaxValue , ErrorMessage = "current stock harus bernilai >= 1")]
    public int? CurrentStock { get; set; }
    
    [JsonPropertyName("last_month_stock")]
    [Required]
    [Range(1 , int.MaxValue , ErrorMessage = "last month stock harus bernilai >= 1")]
    public int? LastMonthStock { get; set; }
    
    [JsonPropertyName("min_stock")]
    [Required]
    [Range(1 , int.MaxValue , ErrorMessage = "min stock harus bernilai >= 1")]
    public int? MinStock { get; set; }
    
    [JsonPropertyName("unit_price")]
    [Required]
    [Range(1 , int.MaxValue , ErrorMessage = "unit price harus bernilai >= 1")]
    public int? UnitPrice { get; set; }

    [JsonPropertyName("uom")] 
    [Required] 
    public string Uom { get; set; } = null!;

    [JsonPropertyName("kategori_id")] 
    [Required]
    [Range(1 , int.MaxValue , ErrorMessage = "tolong pilih kategori")]
    public int? KategoriId { get; set; }

    public Models.Barang ToBarang()
    {
        return new Models.Barang(
            nama: Nama,
            kodeBarang: KodeBarang,
            nomorRak: NomorRak ?? default,
            nomorLaci: NomorLaci ?? default,
            nomorKolom: NomorKolom ?? default,
            kategoriId: KategoriId ?? default,
            currentStock: CurrentStock ?? default,
            lastMonthStock: LastMonthStock ?? default,
            minStock: MinStock ?? default,
            unitPrice: UnitPrice ?? default,
            uom: Uom,
            id: Id
        );
    }
}