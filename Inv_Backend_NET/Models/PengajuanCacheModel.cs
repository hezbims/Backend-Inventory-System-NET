namespace Inventory_Backend_NET.Models;

public class PengajuanCacheModel
{
    public int UrutanHari;
    public DateTime Day;

    public PengajuanCacheModel(
        int urutanHari = 1,
        DateTime? day = null
    )
    {
        UrutanHari = urutanHari;
        Day = day ?? DateTime.Now;
    }
    
    public static PengajuanCacheModel From(string? value)
    {
        if (value == null)
        {
            return new PengajuanCacheModel();
        }

        var splitValue = value.Split('\t');

        return new PengajuanCacheModel(
            urutanHari: int.Parse(splitValue[0]),
            day: DateTime.ParseExact(splitValue[1] , DateFormat  ,  System.Globalization.CultureInfo.InvariantCulture)
        );
    }
    public override string ToString()
    {
        return $"{UrutanHari}\t{Day.ToString(DateFormat)}";
    }

    private const string DateFormat = "yyyy-MM-dd";
}