using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using CsvHelper.Configuration.Attributes;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Barang;
using Inventory_Backend_NET.Service;
using Inventory_Backend_NET.UseCases;
using Inventory_Backend_NET.UseCases.Barang;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StreamReader = System.IO.StreamReader;

namespace Inventory_Backend_NET.Controllers.Barang;


[Route("api/barang/submit-csv")]
public class PostBarangByCsvController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly ValidateBarangPropertyAvailabilityUseCase _barangPropertyValidator;

    public PostBarangByCsvController(
        MyDbContext db
    )
    {
        _db = db;
        _barangPropertyValidator = new ValidateBarangPropertyAvailabilityUseCase(
            db: db    
        );
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
                var errors = ModelState.ToMinimalDictionary();
                return BadRequest(new ErrorModel(errors));
            }

            using (var streamReader = new StreamReader(uploadModel.Csv!.OpenReadStream()))
            using (var csvReader = MyCsvReader.From(streamReader))
            {
                var headerError = csvReader.ValidateHeader();
                if (headerError != null)
                {
                    return BadRequest(new ErrorModel("HEADER" , headerError));
                }
                
                var rows = csvReader
                    .GetRecords<CsvBarangDto>()!
                    .ToList();

                ValidateCsvField(rows);

                if (!ModelState.IsValid)
                    return BadRequest(new ErrorModel(
                        errors: ModelState.ToMinimalDictionary()
                    ));
                
                foreach (var (barangRow , index) in rows.Select((value , i) => (value , i)))
                {
                    var targetUpdateBarang = uploadModel.OverWriteByKodeBarang == true ? 
                        _db.Barangs.FirstOrDefault(
                            barang => barang.KodeBarang == barangRow.KodeBarang    
                        ) : null;
                    var targetKategori = _db.Kategoris.FirstOrDefault(
                        kategori => kategori.Nama == barangRow.NamaKategori
                    );
                    if (targetKategori == null)
                    {
                        targetKategori = new Models.Kategori(nama: barangRow.NamaKategori!);
                    }
                    var newBarang = new Models.Barang(
                        id: targetUpdateBarang?.Id,
                        kodeBarang: barangRow.KodeBarang!,
                        nama: barangRow.NamaBarang!,
                        kategori: targetKategori,
                        minStock: barangRow.MinStock ?? 
                             throw new NoNullAllowedException(
                                 "kesalahan validasi kodingan, min stock kosong"
                             ),
                        nomorRak: barangRow.NomorRak ?? 
                            throw new NoNullAllowedException("kesalahan validasi kodingan, nomor rak kosong"),
                        nomorLaci: barangRow.NomorLaci ??
                            throw new NoNullAllowedException("kesalahan validasi kodingan, nomor laci kosong"),
                        nomorKolom: barangRow.NomorKolom ??  
                            throw new NoNullAllowedException("kesalahan validasi kodingan, nomor kolom kosong"),
                        currentStock: barangRow.CurrentStock ??         
                            throw new NoNullAllowedException(
                                "kesalahan validasi kodingan, current stock kosong"
                            ), 
                        lastMonthStock: barangRow.LastMonthStock ??
                            throw new NoNullAllowedException(
                                "kesalahan validasi kodingan, last month stock kosong"
                            ),
                        unitPrice: barangRow.UnitPrice ??
                           throw new NoNullAllowedException(
                               "kesalahan validasi kodingan, unit price kosong"
                           ),
                        uom: barangRow.Uom!
                    );


                    var isPropertyAvailable = CekNama_KodeBarang_And_Rak_Availability(newBarang , index);
                    
                    if (!isPropertyAvailable)
                    {
                        continue;
                    }

                    if (targetKategori.Id == default)
                    {
                        _db.Add(targetKategori);
                    }

                    if (targetUpdateBarang == null)
                        _db.Barangs.Update(newBarang);
                    else 
                        _db.Barangs.Add(newBarang);
                    _db.SaveChanges();
                }

                if (!ModelState.IsValid)
                {
                    transaction.Rollback();
                    return BadRequest(new ErrorModel(
                        errors: ModelState.ToMinimalDictionary()
                    ));
                }

                transaction.Commit();
                return Ok(new { message = "Sukses" });
            }
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            transaction.Rollback();
            return StatusCode(500, new ErrorModel(
                "Unknown server error" , 
                e.Message)
            );
        }
    }

    private bool CekNama_KodeBarang_And_Rak_Availability(
        Models.Barang newBarang,
        int currentIndex
    )
    {
        var errorDict = _barangPropertyValidator.ValidatePropertyAvailability(
            barangId: newBarang.Id,
            namaProperty: new ValidationProperty<string>(
                property: newBarang.Nama,
                errorKey: "nama",
                errorMessage: "Nama barang terdeteksi berduplikat" 
            ),
            kodeBarangProperty: new ValidationProperty<string>(
                property: newBarang.KodeBarang,
                errorKey: "kode_barang",
                errorMessage: "Kode barang terdeteksi berduplikat"
            ),
            rakProperty: new ValidationProperty<RakDto>(
                property: new RakDto(
                    nomorRak: newBarang.NomorRak,
                    nomorLaci: newBarang.NomorLaci,
                    nomorKolom: newBarang.NomorKolom
                ),
                errorKey: "nomor_rak",
                errorMessage: "Lokasi rak, laci, dan kolom sudah pernah digunakan"
            )
        );
        foreach (var keyValuePair in errorDict)
            ModelState.AddModelError(
                $"BARIS #{currentIndex + 2}" , 
                keyValuePair.Value
            );

        return errorDict.Count() == 0;
    }

    private void ValidateCsvField(List<CsvBarangDto> rows)
    {
        for (var i = 0 ; i < rows.Count ; i++)
        {
            var errorRow = new List<string>();
            if (rows[i].KodeBarang.IsNullOrEmpty()) 
                errorRow.Add("Kode Barang tidak boleh kosong");
            
            if (rows[i].NamaBarang.IsNullOrEmpty()) 
                errorRow.Add("Nama Barang tidak boleh kosong");
            
            if (rows[i].NamaKategori.IsNullOrEmpty()) 
                errorRow.Add("Kategori tidak boleh kosong");
            
            if (!(rows[i].NomorRak >= 1 && rows[i].NomorRak <= Models.Barang.MaxNomorRak)) 
                errorRow.Add($"Nomor Rak harus di rentang 1-{Models.Barang.MaxNomorRak}");
            
            if (!(rows[i].NomorLaci >= 1 && rows[i].NomorLaci <= Models.Barang.MaxNomorLaci)) 
                errorRow.Add($"Nomor Laci harus di rentang 1-{Models.Barang.MaxNomorLaci}");
                
            if (!(rows[i].NomorKolom >= 1 && rows[i].NomorKolom <= Models.Barang.MaxNomorKolom)) 
                errorRow.Add($"Nomor Kolom harus di rentang 1-{Models.Barang.MaxNomorKolom}");
            
            if (!(rows[i].CurrentStock >= 1)) 
                errorRow.Add("Current Stock tidak valid");
            
            if (!(rows[i].MinStock >= 1)) 
                errorRow.Add("Min. Stock tidak valid");
            
            if (!(rows[i].LastMonthStock >= 1)) 
                errorRow.Add("Last Month Stock tidak valid");
            
            if (!(rows[i].UnitPrice >= 1)) 
                errorRow.Add("Unit Price tidak valid");
            
            if (rows[i].Uom.IsNullOrEmpty()) 
                errorRow.Add("UOM tidak boleh kosong");

            if (!errorRow.IsNullOrEmpty())
                foreach (var error in errorRow)
                {
                    ModelState.AddModelError(
                        key: $"BARIS #{i + 2}",
                        errorMessage: error
                    );    
                }
        }
    }

    public class CsvUploadModel
    {
        [FromForm(Name = "csv")]
        [JsonPropertyName(name: "csv")]
        [Required(ErrorMessage = "Tolong pilih file csv anda!")]
        [AllowedFileExtensions(new []{ ".csv"} )]
        public IFormFile? Csv { get; set; }
        
        // Tadi ada defect yag lama banget aku solve, permasalahannya juga gak tau kenapa, di bagian sini.
        [FromForm(Name="overwrite_by_kode_barang")]
        [JsonPropertyName(name: "overwrite_by_kode_barang")]
        [Required(ErrorMessage = "Ada bug di kodingan frontend, " +
                                 "overwrite_by_kode_barang " +
                                 "tidak dispesifikasikan dalam form body")]
        public bool? OverWriteByKodeBarang { get; set; }
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
        public IDictionary<string , List<string>> Errors { get; init; }

        public ErrorModel(string key , string error)
        {
            Errors = new Dictionary<string, List<string>>
            {
                { key , new List<string>{ error } } 
            };
        }

        public ErrorModel(string key , List<string> errors)
        {
            Errors = new Dictionary<string, List<string>>
            {
                { key , errors }
            };
        }

        public ErrorModel(IDictionary<string, List<string>> errors)
        {
            Errors = errors;
        }
    }
}