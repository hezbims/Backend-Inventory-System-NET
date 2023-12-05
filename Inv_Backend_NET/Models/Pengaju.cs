namespace Inventory_Backend_NET.Models;

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