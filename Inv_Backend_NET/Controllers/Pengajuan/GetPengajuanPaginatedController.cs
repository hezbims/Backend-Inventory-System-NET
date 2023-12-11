using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Pengajuan;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Route("api/pengajuan/get")]
public class GetPengajuanPaginatedController : ControllerBase
{
    private MyDbContext _db;

    public GetPengajuanPaginatedController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult GetPengajuanPaginated(
        [FromQuery(Name = "id_pengaju")] int? idPengaju,
        [FromQuery] string keyword,
        [FromQuery] int page
    )
    {
        var result = _db.Pengajuans
            .Include(e => e.User)
            .Include(e => e.Pengaju)
            .OrderBy(pengajuan => pengajuan.CreatedAt)
            .Where(pengajuan => EF.Functions.Like(
                    pengajuan.KodeTransaksi,
                    $"%{keyword}%"
                )
            )
            .OrderByDescending(pengajuan => pengajuan.Id)
            .Paginate(
                pageNumber: page , 
                dataMapper: pengajuan => PengajuanPreviewDto.From(pengajuan)
            );

        return Ok(result);
    }
    
}