using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Dto.Response;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Pengaju.GetPengaju;

[Route("api/pengaju/get")]
public class GetPengajuController : Controller
{

    private MyDbContext _db;
    
    public GetPengajuController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult Get(
        [FromQuery(Name = "is_pemasok")] int intIsPemasok   
    )
    {
        bool isPemasok = intIsPemasok == 1;
        
        var result = _db.Pengajus
            .Where(pengaju => pengaju.IsPemasok == isPemasok)
            .Select(pengaju => PengajuDto.From(pengaju))
            .ToList();

        return Ok(new
        {
            data = result
        });
    }
}