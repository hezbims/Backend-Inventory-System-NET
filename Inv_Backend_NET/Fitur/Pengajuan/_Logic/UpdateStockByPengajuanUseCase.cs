using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Logic;

public class UpdateStockByPengajuanUseCase
{
    private readonly MyDbContext _db;

    public UpdateStockByPengajuanUseCase(MyDbContext db)
    {
        _db = db;
    }

    public void By(
        Database.Models.Pengajuan? previousPengajuan, 
        Database.Models.Pengajuan? currentPengajuan)
    {
        if (previousPengajuan?.Status == StatusPengajuan.Diterima)
        {
            UpdateStockByListBarangAjuan(
                isPemasukan: !previousPengajuan.Pengaju.IsPemasok,
                listBarangAjuan: previousPengajuan.BarangAjuans);
        }
        if (currentPengajuan?.Status == StatusPengajuan.Diterima)
        {
            UpdateStockByListBarangAjuan(
                isPemasukan: currentPengajuan.Pengaju.IsPemasok,
                listBarangAjuan: currentPengajuan.BarangAjuans);
        }
    }

    private void UpdateStockByListBarangAjuan(
        bool isPemasukan,
        ICollection<BarangAjuan> listBarangAjuan)
    {
        foreach (var barangAjuan in listBarangAjuan)
        {
            var quantity = barangAjuan.Quantity;
            if (!isPemasukan)
                quantity *= -1;

            _db.Barangs
                .Where(barang => barang.Id == barangAjuan.BarangId)
                .ExecuteUpdate(s =>
                    s.SetProperty(
                        barang => barang.CurrentStock,
                        barang => barang.CurrentStock + quantity));
        }
    }
}