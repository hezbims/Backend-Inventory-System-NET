using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.Service;

public static class StockBarangUpdateExtension
{
    public static void UpdateStockBarang(
        this MyDbContext db,
        Pengajuan pengajuan,
        bool isUndo
    )
    {
        var isPemasukan = pengajuan.Pengaju.IsPemasok;
        if (isUndo)
        {
            isPemasukan = !isPemasukan;
        }

        var multiplier = isPemasukan ? 1 : -1;

        var updatedBarangIds = pengajuan.BarangAjuans
            .Select(barangAjuan => barangAjuan.BarangId);

        var updatedBarangs = db.Barangs
            .Where(barang => updatedBarangIds.Contains(barang.Id))
            .ToList();
        
        foreach (var barangAjuan in pengajuan.BarangAjuans)
        {
            foreach (var barang in updatedBarangs)
            {
                if (barangAjuan.BarangId == barang.Id)
                {
                    barang.CurrentStock += barangAjuan.Quantity * multiplier;
                }
            }
        }
    }
}