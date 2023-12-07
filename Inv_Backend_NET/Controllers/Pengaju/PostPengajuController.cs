using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Pengaju;

[Route("pengaju/add")]
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
            _db.Pengajus.Add(new Models.Pengaju(
                    nama: requestBody.Nama,
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
        public bool IsPemasok { get; set; }
        
        [JsonPropertyName("nama")]
        public string Nama { get; set; }
    }
}