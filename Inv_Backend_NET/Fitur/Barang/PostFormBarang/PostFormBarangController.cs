using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Barang._Logic;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Barang.PostFormBarang;

[Route("api/barang/add")]
public class PostFormBarangController : Controller
{
    private readonly MyDbContext _db;
    private readonly ValidateBarangPropertyAvailabilityUseCase _propertyAvailabilityValidator;
    
    public PostFormBarangController(MyDbContext db)
    {
        _db = db;
        _propertyAvailabilityValidator = new ValidateBarangPropertyAvailabilityUseCase(db: db);
    }
    
    [HttpPost]
    public IActionResult Index([FromBody] PostBarangDto requestBody)
    {
        try
        {
            var validationDictionary = _propertyAvailabilityValidator.ValidatePropertyAvailability(
                barangId: requestBody.Id,
                namaProperty: new ValidationProperty<string>(
                    property: requestBody.Nama , 
                    errorKey: "nama" , 
                    errorMessage: "Nama sudah terpakai"
                ),
                rakProperty: new ValidationProperty<RakDto>(
                    property: new RakDto(
                        nomorRak: requestBody.NomorRak ?? default, 
                        nomorLaci: requestBody.NomorLaci ?? default,
                        nomorKolom: requestBody.NomorKolom ?? default
                    ),
                    errorKey: "nomor_rak",
                    errorMessage:"Lokasi pada rak, laci, dan kolom ini sudah terpakai"
                ),
                kodeBarangProperty: new ValidationProperty<string>(
                    property: requestBody.KodeBarang,
                    errorKey: "kode_barang",
                    errorMessage: "Kode barang ini sudah terpakai"
                )
            );
            foreach (var keyValuePair in validationDictionary)
                ModelState.AddModelError(keyValuePair.Key , keyValuePair.Value);

            
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToMinimalDictionary();
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
        catch (System.Exception e)
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

    public Database.Models.Barang ToBarang()
    {
        return new Database.Models.Barang(
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