using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Controllers.Barang
{
    [Route("api/barang/all")]
    public class GetBarangPaginatedController : Controller
    {
        private readonly MyDbContext _db;
        public GetBarangPaginatedController(MyDbContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public IActionResult Get(
            [FromQuery(Name = "id_kategori")] int? idKategori,
            [FromQuery] string keyword,
            [FromQuery] int page
        )
        {
            var query = _db.Barangs.Where(barang => 
                EF.Functions.Like(
                    barang.Nama,
                    $"%{keyword}%"
                ) || 
                EF.Functions.Like(
                    barang.KodeBarang,
                    $"%{keyword}%"
                )
            );

            if (idKategori != null)
            {
                query = query.Where(barang =>
                    barang.KategoriId == idKategori
                );
            }

            var result = query
                .Skip((page - 1) * MyConstants.PageSize)
                .Take(MyConstants.PageSize)
                .ToList();
            
            int totalData = result.Count();
            if (totalData > 0)
                result.RemoveAt(totalData - 1);
            return Ok(new { data = result });
        }
    }
}
