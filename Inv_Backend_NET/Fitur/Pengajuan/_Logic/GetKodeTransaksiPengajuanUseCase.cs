using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Constants;

namespace Inventory_Backend_NET.Fitur.Pengajuan._Logic;

public class GetKodeTransaksiPengajuanUseCase
{
    private readonly MyDbContext _db;

    public GetKodeTransaksiPengajuanUseCase(MyDbContext db)
    {
        _db = db;
    }
    
    public string Run(
        DateTimeOffset dateCreatedAt,
        Database.Models.Pengaju pengaju
    )
    {
        var tanggalPengajuan = dateCreatedAt.ToString(MyConstants.DateFormat);
        var kodeUrutan = GetKodeUrutan(tanggalPengajuan: tanggalPengajuan);
        var tipePengajuan = pengaju.IsPemasok ? "IN" : "OUT";

        return $"TRX-{tipePengajuan}-{tanggalPengajuan}-{kodeUrutan}";
    }

    private string GetKodeUrutan(
        string tanggalPengajuan    
    )
    {
        var totalPengajuanByTanggal = _db.TotalPengajuanByTanggals.FirstOrDefault(
            data => data.Tanggal == tanggalPengajuan
        );
        if (totalPengajuanByTanggal == null)
        {
            return "001";
        }

        var urutan = totalPengajuanByTanggal.Total + 1;
        return urutan.ToString().PadLeft(3 , '0');
    }
}