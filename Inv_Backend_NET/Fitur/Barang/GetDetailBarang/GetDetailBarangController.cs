using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur.Barang._Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Barang.GetDetailBarang;

[Route("api/barang/detail")]
public class GetDetailBarangController : Controller
{
    private MyDbContext _db;
    public GetDetailBarangController(
        MyDbContext db    
    )
    {
        _db = db;
    }
    
    [HttpGet]
    [Route("{barangId}")]
    public IActionResult Get(
        [FromRoute] int barangId
    )
    {
        try
        { 
            var currentBarang = _db.Barangs
                .Include(barang => barang.Kategori)
                .Single(barang => barang.Id == barangId);

            var result = BarangDto.From(currentBarang);

            return Ok(new
            {
                data = result
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