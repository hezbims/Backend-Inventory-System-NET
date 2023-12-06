using Inventory_Backend_NET.DTO;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Kategori;

[Route("api/kategori/all")]
public class GetAllKategoriController : ControllerBase
{
    private MyDbContext _db;
    
    public GetAllKategoriController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult Index()
    {
        var results = _db.Kategoris
            .ToList()
            .Select(kategori => KategoriDto.From(kategori));
        return Ok(new { data =  results});
    }
}