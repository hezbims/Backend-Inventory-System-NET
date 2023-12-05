namespace Inventory_Backend_NET.Models;

public class UrutanPengajuanCacheModel
{
    public int UrutanHari;
    public DateTime Day;

    public UrutanPengajuanCacheModel(
        int urutanHari = 1,
        DateTime? day = null
    )
    {
        UrutanHari = urutanHari;
        Day = day ?? DateTime.Now;
    }
    
    public static UrutanPengajuanCacheModel From(string? value)
    {
        if (value == null)
        {
            return new UrutanPengajuanCacheModel();
        }

        var splitValue = value.Split('\t');

        return new UrutanPengajuanCacheModel(
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