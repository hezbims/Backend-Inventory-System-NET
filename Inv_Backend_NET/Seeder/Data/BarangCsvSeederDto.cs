using CsvHelper.Configuration.Attributes;
using Inventory_Backend_NET.DTO.Barang;

namespace Inventory_Backend_NET.Seeder.Data;

public class BarangCsvSeederDto
{
    [Name("ITEM NO")] public string ItemNo { get; set; } = null!;

    [Name("ITEM DESCRIPTION")] public string ItemDescription { get; set; } = null!;

    [Name("LOCATION")] public string Location { get; set; } = null!;

    [Name("UOM")] public string Uom { get; set; } = null!;

    [Name("STD STOCK")] public int MinStock { get; set; }

    [Name("LAST MONTH STOCK")] public int? LastMonthStock { get; set; }

    [Name("ACTUAL")] public int? Actual { get; set; }

    [Name("UNIT PRICE")] public string UnitPrice { get; set; } = null!;

    public int IntUnitPrice
    {
        get
        {
            try
            {
                var withoutRupiah = UnitPrice.Split(' ')[1];
                var result = withoutRupiah.Replace(",", "");

                return int.Parse(result);
            }
            catch (Exception)
            {
                return 0;
            }
        }
    }

    private static int _counter = 1;

    public RakDto Rak
    {
        get
        {
            var kodeRak = ItemNo.Remove(0, 1).Split('-');
            return new RakDto(
                nomorRak : int.Parse(kodeRak[0]),
                nomorLaci : int.Parse(kodeRak[1]),
                nomorKolom : _counter++
            );
        }
    }
}