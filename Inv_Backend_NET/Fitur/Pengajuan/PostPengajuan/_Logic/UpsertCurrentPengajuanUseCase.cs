using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Fitur.Pengajuan.PostPengajuan._Logic;

public class UpsertCurrentPengajuanUseCase
{
    private readonly MyDbContext _db;
    private readonly TimeProvider _timeProvider;

    public UpsertCurrentPengajuanUseCase(
        MyDbContext db, 
        TimeProvider timeProvider
    )
    {
        _db = db;
        _timeProvider = timeProvider;
    }
    
    public Database.Models.Pengajuan By(
        Database.Models.Pengajuan? previousPengajuan,
        SubmitPengajuanBody requestBody,
        User submitter
    )
    {
        var currentPengaju = _db.Pengajus.Single(pengaju => pengaju.Id == requestBody.IdPengaju);

        
        var currentBarangAjuans = requestBody.BarangAjuans!.Select(
            barangAjuanBody => barangAjuanBody.ToBarangAjuan()
        ).ToList();

        if (currentBarangAjuans.Count == 0)
        {
            throw new BadHttpRequestException("Tolong ajukan minimal satu barang!");
        }

        
        User pemilikPengajuan = previousPengajuan?.User ?? submitter;

        
        StatusPengajuan statusPengajuan;

        int totalQuantityOfCurrentPengajuan = requestBody.BarangAjuans!.Sum(barangAjuan => barangAjuan.Quantity);
        if (!submitter.IsAdmin && totalQuantityOfCurrentPengajuan == 0)
            throw new BadHttpRequestException("Tidak boleh mengajukan dengan total quantity 0");
        if (previousPengajuan != null && totalQuantityOfCurrentPengajuan == 0)
            statusPengajuan = StatusPengajuan.Ditolak;
        else
            statusPengajuan = StatusPengajuan.GetByEditor(submitter);

        Database.Models.Pengajuan currentPengajuan = new Database.Models.Pengajuan(
            db: _db,
            pengaju: currentPengaju,
            status: statusPengajuan,
            user: pemilikPengajuan,
            barangAjuans: currentBarangAjuans,
            id: previousPengajuan?.Id,
            timeProvider: _timeProvider,
            createdAt: previousPengajuan?.WaktuPengajuan
        );
            
        if (previousPengajuan == null)
        {
            _db.Pengajuans.Add(currentPengajuan);
        }
        else
        {
            _db.BarangAjuans.RemoveRange(previousPengajuan.BarangAjuans);
            _db.Entry(previousPengajuan).State = EntityState.Detached;

            currentPengajuan.KodeTransaksi = previousPengajuan.KodeTransaksi;
            _db.Pengajuans.Update(currentPengajuan);
        }

        return currentPengajuan;
    }
}