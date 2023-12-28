using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur.Pengajuan.GetDetailPengajuan._Dto.Response;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Pengajuan.GetDetailPengajuan;

[Route("api/pengajuan/get")]
public class GetDetailPengajuanController : ControllerBase
{

    private readonly MyDbContext _db;

    public GetDetailPengajuanController(MyDbContext db)
    {
        _db = db;
    }
    
    [Route("{idPengajuan}")]
    [HttpGet]
    public IActionResult Index(int idPengajuan)
    {
        try
        {
            var pengajuan = _db.Pengajuans
                .Include(pengajuan => pengajuan.Pengaju)
                .Include(pengajuan => pengajuan.BarangAjuans)
                .ThenInclude(barangAjuan => barangAjuan.Barang)
                .Single(pengajuan => pengajuan.Id == idPengajuan);

            var result = DetailPengajuanDto.From(pengajuan);
            return Ok(new
            {
                data = result
            });

        }
        catch (Exception e)
        {
            return StatusCode(500, new
            {
                message = e.Message 
            });
        }
    }
}