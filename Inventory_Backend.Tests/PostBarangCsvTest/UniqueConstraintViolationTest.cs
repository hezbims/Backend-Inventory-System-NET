using Inventory_Backend_NET.Controllers.Barang;
using Inventory_Backend_NET.Models;
using Inventory_Backend.Tests.TestConfiguration;
using Inventory_Backend.Tests.TestConfiguration.Constant;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend.Tests.PostBarangCsvTest;

[Collection(TestConstant.WithDbCollection)]
public class UniqueConstraintViolationTest : IDisposable
{
    private readonly MyDbFixture _fixture;
    public UniqueConstraintViolationTest(MyDbFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public void Test_Non_Overwrite_Kode_Barang_Nama_Dan_Lokasi_Konflik_Di_Csv_Yang_Sama()
    {
        var formFile = TestAssetsUtils.GetFormFile(
            filename: "dua_barang_sama_gagal_overwrite.csv" , 
            jsonKey: "csv"
        );

        var db = _fixture.CreateContext();
        var controller = new PostBarangByCsvController(db);

        var actionResult = (controller.Index(new PostBarangByCsvController.CsvUploadModel
        {
            Csv = formFile,
            OverWriteByKodeBarang = false
        }) as BadRequestObjectResult)!;

        var errorModel = (actionResult.Value as PostBarangByCsvController.ErrorModel)!;
        var baris3 = errorModel.Errors["BARIS #3"];
        var namaBarang = "Mata Solder";
        Assert.Contains(
            $"Nama barang terdeteksi berduplikat (barang={namaBarang})", 
            baris3
        );
        Assert.Contains(
            $"Lokasi rak, laci, dan kolom sudah pernah digunakan (barang={namaBarang})", 
            baris3
        );
        Assert.Contains(
            $"Kode barang terdeteksi berduplikat (barang={namaBarang})", 
            baris3
        );
        Assert.Empty(db.Barangs.ToList());
        
    }

    [Fact]
    public void Test_Mau_Overwrite_Tapi_Konflik_Dengan_Yang_Sudah_Ada_Di_Database()
    {
        var db = _fixture.CreateContext();
        
        var kategori = new Kategori(nama: "Only-kategori");
        db.Kategoris.Add(kategori);
        
        var barang1 = new Barang(
            kodeBarang: "R1-1-1",
            nama: "Solder-1",   
            kategori: kategori,
            minStock: 1,
            nomorRak: 1,
            nomorLaci: 1,
            nomorKolom: 1,
            currentStock: 1,
            lastMonthStock: 1,
            unitPrice: 1,
            uom: "Pc"
        );
        
        var barang2 = new Barang(
            kodeBarang: "R1-1-2",
            nama: "Solder-2",
            kategori: kategori,
            minStock: 1,
            nomorRak: 1,
            nomorLaci: 1,
            nomorKolom: 2,
            currentStock: 1,
            lastMonthStock: 1,
            unitPrice: 1,
            uom: "Pc"
        );

        db.Barangs.Add(barang1);
        db.Barangs.Add(barang2);
        db.SaveChanges();

        var fileForm = TestAssetsUtils.GetFormFile(
            filename: "try_overwrite_but_conflict_with_database.csv",
            jsonKey: "csv"
        );

        var controller = new PostBarangByCsvController(db);

        var actionResult = (controller.Index(new PostBarangByCsvController.CsvUploadModel
        {
            Csv = fileForm,
            OverWriteByKodeBarang = true
        }) as BadRequestObjectResult)!;


        var errorModel = (actionResult.Value as PostBarangByCsvController.ErrorModel)!;
        var baris3 = errorModel.Errors["BARIS #2"];
        var namaBarang = "Solder-2";
        Assert.Contains(
            $"Nama barang terdeteksi berduplikat (barang={namaBarang})", 
            baris3
        );
        Assert.Contains(
            $"Lokasi rak, laci, dan kolom sudah pernah digunakan (barang={namaBarang})", 
            baris3
        );

        var allBarangs = db.Barangs
            .Include(barang => barang.Kategori)
            .ToList();
        Assert.Single(
            allBarangs.Where(
                barang => 
                    barang.KodeBarang == barang1.KodeBarang &&
                    barang.Kategori.Nama == barang1.Kategori.Nama &&
                    barang.Nama == barang1.Nama &&
                    barang.MinStock == barang1.MinStock &&
                    barang.NomorRak == barang1.NomorRak &&
                    barang.NomorLaci == barang1.NomorLaci &&
                    barang.NomorKolom == barang1.NomorKolom &&
                    barang.LastMonthStock == barang1.LastMonthStock &&
                    barang.CurrentStock == barang1.CurrentStock &&
                    barang.UnitPrice == barang1.UnitPrice &&
                    barang.Uom == barang1.Uom
            )
        );
        Assert.Single(
            allBarangs.Where(
                barang => 
                    barang.KodeBarang == barang2.KodeBarang &&
                    barang.Kategori.Nama == barang2.Kategori.Nama &&
                    barang.Nama == barang2.Nama &&
                    barang.MinStock == barang2.MinStock &&
                    barang.NomorRak == barang2.NomorRak &&
                    barang.NomorLaci == barang2.NomorLaci &&
                    barang.NomorKolom == barang2.NomorKolom &&
                    barang.LastMonthStock == barang2.LastMonthStock &&
                    barang.CurrentStock == barang2.CurrentStock &&
                    barang.UnitPrice == barang2.UnitPrice &&
                    barang.Uom == barang2.Uom
            )
        );
        Assert.Equal(2 , allBarangs.Count);
    }
    

    public void Dispose()
    {
        _fixture.Cleanup();
    }
}