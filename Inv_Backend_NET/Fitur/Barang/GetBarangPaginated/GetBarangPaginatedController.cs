using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur.Barang._Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Barang.GetBarangPaginated
{
    // TODO : Test kalo current stock yang direturn benar
    [Route("api/barang/all")]
    [Authorize(policy: MyConstants.Policies.AllUsers)]
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
            try
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

                if (idKategori != null && idKategori != 0)
                {
                    query = query.Where(barang =>
                        barang.KategoriId == idKategori
                    );
                }

                var result = query
                    .Include(barang => barang.Kategori)
                    .OrderByDescending(barang => barang.Id)
                    .Paginate(pageNumber: page)
                    .MapTo(mapper: barang => 
                        BarangDto.From(barang) 
                    );

                return Ok(result);
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
}
