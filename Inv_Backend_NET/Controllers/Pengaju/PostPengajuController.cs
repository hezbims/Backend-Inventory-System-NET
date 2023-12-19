using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.UseCases.Common;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Pengaju;

[Route("api/pengaju/add")]
public class PostPengajuController : ControllerBase
{
    private readonly MyDbContext _db;
    public PostPengajuController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpPost]
    public IActionResult Index(
        [FromBody] PostNewPengajuDto requestBody    
    )
    {
        try
        {
            var existedPengaju = _db.Pengajus.FirstOrDefault(pengaju => pengaju.Nama == requestBody.Nama);
            if (existedPengaju != null)
                ModelState.AddModelError("nama" , "Pengaju ini sudah ada dalam database!");

            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    errors = ModelState.ToMinimalDictionary()
                });
            
            _db.Pengajus.Add(new Models.Pengaju(
                    nama: requestBody.Nama!,
                    isPemasok: requestBody.IsPemasok
                )
            );
            _db.SaveChanges();
            return Ok(new
            {
                message = "sukses"
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500 , new
            {
                message = e.Message
            });
        }
    }

    public class PostNewPengajuDto
    {
        [JsonPropertyName("is_pemasok")]
        [Required]
        public bool IsPemasok { get; set; }
        
        [JsonPropertyName("nama")]
        [Required]
        public string? Nama { get; set; }
    }
}