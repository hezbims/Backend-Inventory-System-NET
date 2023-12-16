using Inventory_Backend_NET.Database;

namespace Inventory_Backend_NET.UseCases.Common;

public class UpdateStockUseCase
{
    private readonly MyDbContext _db;

    public UpdateStockUseCase(MyDbContext db)
    {
        _db = db;
    }
    
    public void By(
        Models.Pengajuan? previousPengajuan,
        Models.Pengajuan? currentPengajuan
    )
    {
        var previousModifiedBarangId =
            previousPengajuan?.BarangAjuans.Select(barangAjuan => barangAjuan.BarangId).ToArray() ?? new int[]{};
        var currentModifiedBarangId =
            currentPengajuan?.BarangAjuans.Select(barangAjuan => barangAjuan.BarangId).ToArray() ?? new int[]{};

        var allModifiedBarangId = new List<int>();
        allModifiedBarangId.AddRange(previousModifiedBarangId);
        allModifiedBarangId.AddRange(currentModifiedBarangId);

        var willModifiedBarangs = _db.Barangs
            .Where(barang => allModifiedBarangId.Contains(barang.Id))
            .ToArray();
        var invalidBarang = willModifiedBarangs 
            .FirstOrDefault(barang => UpdateStockAndFilterInvalidBarang(
                barang: barang, 
                previousPengajuan: previousPengajuan , 
                currentPengajuan: currentPengajuan    
            ));
        
        if (invalidBarang != null)
        {
            throw new BadHttpRequestException(
                message: "Operasi tidak valid :\n" +
                         $"quantity dari barang {invalidBarang.Nama}," +
                         $"menjadi {invalidBarang.CurrentStock}"
            );
        }
        _db.SaveChanges();
    }

    private bool UpdateStockAndFilterInvalidBarang(
        Models.Barang barang,
        Models.Pengajuan? previousPengajuan, 
        Models.Pengajuan? currentPengajuan
    )
    {
        if (previousPengajuan != null)
        {
            var previousMultiplier = previousPengajuan.Pengaju.IsPemasok ? 1 : -1;
            foreach (var barangAjuan in previousPengajuan.BarangAjuans)
                if (barang.Id == barangAjuan.BarangId)
                    barang.CurrentStock -= barangAjuan.Quantity * previousMultiplier;
        }

        if (currentPengajuan != null)
        {
            var currentMultiplier = currentPengajuan.Pengaju.IsPemasok ? 1 : -1;
            foreach (var barangAjuan in currentPengajuan.BarangAjuans)
                if (barang.Id == barangAjuan.BarangId)
                    barang.CurrentStock += barangAjuan.Quantity * currentMultiplier;
        }

        return barang.CurrentStock < 0;
    }
}