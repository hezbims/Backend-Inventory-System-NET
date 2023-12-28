
namespace Inventory_Backend_NET.Database.Models;

public class StatusPengajuan
{
    public int Id { get; set; }
    public string Value { get; set; }

    public StatusPengajuan() { }

    public StatusPengajuan(string value, int id)
    {
        Value = value;
        Id = id;
    }

    public const string DiterimaValue = "diterima";
    public const string MenungguValue = "menunggu";
    public const string DitolakValue = "ditolak";
    
        
    public static StatusPengajuan Diterima = new StatusPengajuan(value: "diterima", id: 1);
    public static StatusPengajuan Menunggu = new StatusPengajuan(value: "menunggu", id: 2);
    public static StatusPengajuan Ditolak = new StatusPengajuan(value: "ditolak", id: 3);
    

    public static StatusPengajuan From(string str)
    {
        switch (str)
        {
            case DiterimaValue: return Diterima;
            case MenungguValue: return Menunggu;
            case DitolakValue: return Ditolak;
            default: throw new Exception($"Value {str} tidak ditemukan pada status pengajuan");
        }
    }

    /// <summary>
    ///  Mendapatkan pengajuan berdasarkan role user yang sedang mengedit atau membuat suatu pengajuan
    /// </summary>
    /// <param name="editor"></param>
    /// <returns></returns>
    public static StatusPengajuan GetByEditor(User editor)
    {
        return editor.IsAdmin ? Diterima : Menunggu;
    }
}


