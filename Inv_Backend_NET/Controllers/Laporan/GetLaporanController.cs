using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Laporan;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Laporan;

[Route("api/kategori/laporan")]
public class GetLaporanController : ControllerBase
{
    private readonly MyDbContext _db;
    public GetLaporanController(
        MyDbContext db
    )
    {
        _db = db;
    }
    
    [HttpGet]
    public IActionResult Index(
        [FromQuery(Name = "month")] int bulan,
        [FromQuery(Name = "year")] int tahun
    )
    {
        var barangAjuanQuery = 
            from barangAjuan in _db.BarangAjuans
            join pengajuan in _db.Pengajuans
                on barangAjuan.PengajuanId
                equals pengajuan.Id
            join pengaju in _db.Pengajus
                on pengajuan.PengajuId equals pengaju.Id
            select new
            {
                barangAjuan.BarangId,
                quantity = (
                    pengaju.IsPemasok ? barangAjuan.Quantity : -barangAjuan.Quantity
                )
            };
        
        var barangQuery =
            from barang in _db.Barangs
            join barangAjuan in barangAjuanQuery
            on barang.Id equals barangAjuan.BarangId
            group barangAjuan by new {
                barang.Id,
                barang.NomorRak, 
                barang.NomorLaci, 
                barang.NomorKolom, 
                barang.Nama, 
                barang.Uom, 
                barang.MinStock, 
                StockSekarang = barang.CurrentStock, 
                barang.UnitPrice, 
                barang.KodeBarang,
                barang.KategoriId
            }
            into aggregatedBarangAjuan
            orderby aggregatedBarangAjuan.Key.Nama descending 
            select new BarangLaporanDto {
                Id = aggregatedBarangAjuan.Key.Id,
                KategoriId = aggregatedBarangAjuan.Key.KategoriId,
                NomorRak = aggregatedBarangAjuan.Key.NomorRak,
                NomorLaci = aggregatedBarangAjuan.Key.NomorLaci,
                NomorKolom = aggregatedBarangAjuan.Key.NomorKolom,
                Nama = aggregatedBarangAjuan.Key.Nama,
                Uom = aggregatedBarangAjuan.Key.Uom,
                MinStock = aggregatedBarangAjuan.Key.MinStock,
                StockSekarang = aggregatedBarangAjuan.Key.StockSekarang,
                UnitPrice = aggregatedBarangAjuan.Key.UnitPrice,
                KodeBarang = aggregatedBarangAjuan.Key.KodeBarang,
                Out = aggregatedBarangAjuan.Sum(
                    barangAjuan => barangAjuan.quantity < 0 ?
                        -barangAjuan.quantity : 0
                ),
                In = aggregatedBarangAjuan.Sum(
                    barangAjuan => barangAjuan.quantity > 0 ?
                        barangAjuan.quantity : 0
                )
            };

        var kategoriQuery =
            from kategori in _db.Kategoris
            join barang in barangQuery
                on kategori.Id equals barang.KategoriId
            orderby kategori.Nama descending
            group barang by kategori.Nama
            into aggregatedBarang
            select new GetLaporanDto
            {
                NamaKategori = aggregatedBarang.Key,
                Barangs = aggregatedBarang.ToList()
            };
            
        

        return Ok(kategoriQuery);
    }
}