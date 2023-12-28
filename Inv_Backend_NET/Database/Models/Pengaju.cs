using Inventory_Backend_NET.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Database.Models;

[EntityTypeConfiguration(typeof(PengajuConfiguration))]
public class Pengaju
{
    public int Id { get; set; }
    public string Nama { get; set; } = null!;
    public bool IsPemasok { get; set; }

    public Pengaju(string nama, bool isPemasok)
    {
        Nama = nama;
        IsPemasok = isPemasok;
    }
    private Pengaju() { }
}