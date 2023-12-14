namespace Inventory_Backend_NET.DTO.Barang;

public class RakDto
{
    public int NomorRak { get; set; }
    public int NomorLaci { get; set; }
    public int NomorKolom { get; set; }

    public RakDto(int nomorRak, int nomorLaci, int nomorKolom)
    {
        NomorRak = nomorRak;
        NomorLaci = nomorLaci;
        NomorKolom = nomorKolom;
    }
    
}