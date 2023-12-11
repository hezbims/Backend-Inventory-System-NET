using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Kategori;

[Route("kategori/add")]
public class PostNewKategoriController : Controller
{
    private readonly MyDbContext _db;

    public PostNewKategoriController(
        MyDbContext db
    )
    {
        _db = db;
    }
    
    [HttpPost]
    public IActionResult Index([FromBody] PostKategoriDto requestBody)
    {
        try
        {
            _db.Kategoris.Add(requestBody.ToKategori());
            _db.SaveChanges();
            return Ok(new
            {
                message = "Sukses"
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

    public class PostKategoriDto
    {
        [JsonPropertyName("nama")] 
        [Required] 
        public string Nama { get; set; } = null!;

        public Models.Kategori ToKategori()
        {
            return new Models.Kategori(nama: Nama);
        }
    }
}