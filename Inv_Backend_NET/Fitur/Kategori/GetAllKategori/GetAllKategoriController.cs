using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Kategori.GetAllKategori;

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