using Inventory_Backend_NET.Constants;
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

    GetPengajuanPaginatedController(MyDbContext db)
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
        var query = _db.Pengajuans
            .Include(e => e.User)
            .Include(e => e.Status)
            .Include(e => e.Pengaju)
            .OrderBy(pengajuan => pengajuan.CreatedAt)
            .Select(pengajuan => PengajuanPreviewDto.From(pengajuan))
            .Where(pengajuan => EF.Functions.Like(
                    pengajuan.KodeTransaksi,
                    $"%{keyword}%"
                )
            )
            .Skip((page - 1) * MyConstants.PageSize)
            .Take(MyConstants.PageSize + 1);

        return this.Paginate(query.ToList());
    }
    
}