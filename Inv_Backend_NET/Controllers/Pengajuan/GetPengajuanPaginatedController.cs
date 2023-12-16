using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Pengajuan;
using Inventory_Backend_NET.Extension;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Controllers.Pengajuan;

[Route("api/pengajuan/get")]
public class GetPengajuanPaginatedController : ControllerBase
{
    private readonly MyDbContext _db;
    private readonly IHttpContextAccessor _httpContext;

    public GetPengajuanPaginatedController(
        MyDbContext db,
        IHttpContextAccessor httpContext
    )
    {
        _db = db;
        _httpContext = httpContext;
    }
    
    [HttpGet]
    public IActionResult GetPengajuanPaginated(
        [FromQuery(Name = "id_pengaju")] int? idPengaju,
        [FromQuery] string keyword,
        [FromQuery] int page
    )
    {
        try
        {
            var user = _db.GetCurrentUserFrom(_httpContext)!;

            var query = _db.Pengajuans
                .Include(e => e.User)
                .Include(e => e.Pengaju)
                .OrderBy(pengajuan => pengajuan.CreatedAt)
                .Where(pengajuan => EF.Functions.Like(
                        pengajuan.KodeTransaksi,
                        $"%{keyword}%"
                    )
                );

            if (!user.IsAdmin)
            {
                query = query.Where(
                    pengajuan => pengajuan.UserId == user.Id
                );
            }

            if (idPengaju != null)
            {
                query = query.Where(
                    pengajuan => pengajuan.PengajuId == idPengaju
                );
            }

            var result = query
                .OrderByDescending(pengajuan => pengajuan.Id)
                .Paginate(pageNumber: page)
                .MapTo(mapper: pengajuan => PengajuanPreviewDto.From(pengajuan));


            return Ok(result);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, new { message = "Internal Server Error" });
        }
    }
    
}