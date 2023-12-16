using Inventory_Backend_NET.Controllers.Pengajuan;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.UseCases.PostPengajuan;

public class GetEventTypeUseCase
{
    public PengajuanEvent? Exec(
        Pengajuan? previousPengajuan,
        User submitter        
    )
    {
        // Kalo sekarang ini lagi ngebuat pengajuan baru
        if (previousPengajuan == null)
        {
            if (!submitter.IsAdmin)
            {
                return PengajuanEvent.UserNotifAdmin;
            }
        }
        // Kalo lagi ngedit pengajuan sebelumnya
        else
        {
            // Kalo yang mensubmit sekarang bukan admin
            if (!submitter.IsAdmin) 
            {
                // Non-Admin enggak bisa ngedit pengajuan yang bukan miliknya
                if (previousPengajuan.User.Id != submitter.Id)
                {
                    throw new BadHttpRequestException(
                        "Pengajuan ini bukan milik anda"
                    );
                }

                // Non-Admin enggak bisa ngedit pengajuan yang udah ditolak atau diterima
                if (previousPengajuan.Status.Value != StatusPengajuan.MenungguValue)
                {
                    throw new BadHttpRequestException(
                        "Pengajuan yang sudah dikonfirmasi tidak dapat diedit"
                    );
                }

                return PengajuanEvent.UserNotifAdmin;
            }
            else if (previousPengajuan.Status.Value == StatusPengajuan.MenungguValue)
            {
                return PengajuanEvent.AdminNotifUser;
            }
        }

        return null;
    }
}