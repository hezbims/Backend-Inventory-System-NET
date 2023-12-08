using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Barang;

[Route("api/barang/add")]
public class PostFormBarangController : Controller
{
    private readonly MyDbContext _db;
    
    public PostFormBarangController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpPost]
    public IActionResult Index([FromBody] PostBarangDto requestBody)
    {
        try
        {
            ValidateLocationAvailability(requestBody);

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
            return StatusCode(500, new
            {
                message = e.Message
            });
        }
    }

    /// <summary>
    ///  Memvalidasi apakah lokasi yang dipilih sudah digunakan oleh barang lain atau belum
    /// </summary>
    /// <param name="requestBody"></param>
    void ValidateLocationAvailability(PostBarangDto requestBody)
    {
        var barangInCurrentLocation = _db.Barangs
            .FirstOrDefault(
                barang =>
                    barang.NomorRak == requestBody.NomorRak &&
                    barang.NomorLaci == requestBody.NomorLaci &&
                    barang.NomorKolom == requestBody.NomorKolom
            );

        var errorKey = "nomor_rak";
        var errorMessage = "Lokasi pada rak, laci, dan kolom ini sudah terpakai";
        
        if (requestBody.Id == null)
        {
            if (barangInCurrentLocation != null)
            {
                ModelState.AddModelError(
                    key: errorKey, 
                    errorMessage: errorMessage
                );
            }
        }
        else
        {
            if (barangInCurrentLocation?.Id != requestBody.Id)
            {
                ModelState.AddModelError(
                    key: errorKey,
                    errorMessage: errorMessage
                );
            }
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
    
    [JsonPropertyName("nomor_rak")]
    [Range(1, 6)]
    public int NomorRak { get; set; }
    
    [JsonPropertyName("nomor_laci")]
    [Range(1, 30)]
    public int NomorLaci { get; set; }
    
    [JsonPropertyName("nomor_kolom")]
    [Range(1, 9)]
    public int NomorKolom { get; set; }
    
    [JsonPropertyName("stock_sekarang")]
    [Range(1 , int.MaxValue , ErrorMessage = "current stock harus bernilai >= 1")]
    public int CurrentStock { get; set; }
    
    [JsonPropertyName("last_month_stock")]
    [Range(1 , int.MaxValue , ErrorMessage = "last month stock harus bernilai >= 1")]
    public int LastMonthStock { get; set; }
    
    [JsonPropertyName("min_stock")]
    [Range(1 , int.MaxValue , ErrorMessage = "min stock harus bernilai >= 1")]
    public int MinStock { get; set; }
    
    [JsonPropertyName("unit_price")]
    [Range(1 , int.MaxValue , ErrorMessage = "unit price harus bernilai >= 1")]
    public int UnitPrice { get; set; }

    [JsonPropertyName("uom")] 
    [Required] 
    public string Uom { get; set; } = null!;

    [JsonPropertyName("kategori_id")] 
    [Range(1 , int.MaxValue , ErrorMessage = "tolong pilih kategori")]
    public int KategoriId { get; set; }

    public Models.Barang ToBarang()
    {
        return new Models.Barang(
            nama: Nama,
            nomorRak: NomorRak,
            nomorLaci: NomorLaci,
            nomorKolom: NomorKolom,
            kategoriId: KategoriId,
            currentStock: CurrentStock,
            lastMonthStock: LastMonthStock,
            minStock: MinStock,
            unitPrice: UnitPrice,
            uom: Uom,
            id: Id
        );
    }
}