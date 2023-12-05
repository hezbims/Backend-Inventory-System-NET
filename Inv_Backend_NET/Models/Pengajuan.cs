namespace Inventory_Backend_NET.Models;

public class Pengajuan
{
    public int Id { get; set; }
    public long CreatedAt { get; set; }
    public Pengaju Pengaju { get; set; } = null!;
    public int PengajuId { get; set; }
    public int UrutanHariIni { get; set; }
    public StatusPengajuan Status { get; set; } = null!;
    public User User { get; set; } = null!;
    public int UserId { get; set; }

    public Pengajuan()
    {
        DateTime currentTime = DateTime.UtcNow;
        CreatedAt = ((DateTimeOffset)currentTime).ToUnixTimeMilliseconds();
    }
}