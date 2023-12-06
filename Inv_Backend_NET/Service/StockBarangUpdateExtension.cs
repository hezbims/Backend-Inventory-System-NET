using Inventory_Backend_NET.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Service;

public static class StockBarangUpdateExtension
{
    public static void UpdateStockBarang(
        this Pengajuan pengajuan,
        bool isReverse
    )
    {
        var isPemasukan = pengajuan.Pengaju.IsPemasok;
        if (isReverse)
        {
            isPemasukan = !isPemasukan;
        }

        var multiplier = isPemasukan ? 1 : -1;

        var listBarangAjuanId = pengajuan.BarangAjuans.Select(barangAjuan => barangAjuan.Id);

        foreach (var barangAjuan in pengajuan.BarangAjuans)
        {
            barangAjuan.Barang.CurrentStock += barangAjuan.Quantity * multiplier;
        }
    }
}