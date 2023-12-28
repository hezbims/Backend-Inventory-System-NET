using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.UseCases.Common;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Kategori.PostNewKategori;

[Route("api/kategori/add")]
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
            var existedKategori = _db.Kategoris.FirstOrDefault(kategori => kategori.Nama == requestBody.Nama);
            if (existedKategori != null)
                ModelState.AddModelError("nama" , "Kategori ini sudah ada pada database!");

            if (!ModelState.IsValid)
                return BadRequest(new {
                    errors = ModelState.ToMinimalDictionary() 
                });
            
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
        public string? Nama { get; set; }

        public Database.Models.Kategori ToKategori()
        {
            return new Database.Models.Kategori(nama: Nama!);
        }
    }
}