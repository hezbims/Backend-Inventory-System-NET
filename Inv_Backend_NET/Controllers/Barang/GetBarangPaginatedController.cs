﻿using Inventory_Backend_NET.DTO.Barang;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Utils;
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

                if (idKategori != null)
                {
                    query = query.Where(barang =>
                        barang.KategoriId == idKategori
                    );
                }

                var result = query
                    .Include(barang => barang.Kategori)
                    .Select(barang => BarangDto.From(barang))
                    .Paginate(pageNumber: page);

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
