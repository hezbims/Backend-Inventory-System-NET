using System.Globalization;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Service;

public class MyCsvReader : CsvReader
{
    private MyCsvReader(
        StreamReader streamReader,
        CsvConfiguration config
    ) : base(streamReader , config)
    { }

    public static MyCsvReader From(StreamReader streamReader)
    {
        var config = new CsvConfiguration(CultureInfo.InvariantCulture)
        {
            PrepareHeaderForMatch = arg => arg.Header.ToUpper(),
        };

        var csvReader = new MyCsvReader(streamReader, config);
        return csvReader;
    }

    public string? ValidateHeader()
    {
        var expectedHeaders = new[]
        {
            "KODE BARANG",
            "NAMA BARANG",
            "KATEGORI",
            "NOMOR RAK",
            "NOMOR LACI",
            "NOMOR KOLOM",
            "CURRENT STOCK",
            "MIN. STOCK",
            "LAST MONTH STOCK",
            "UNIT PRICE",
            "UOM"
        };

        Read();
        ReadHeader();
        var currentHeaders = Context.Reader.HeaderRecord?.Select(
            header => header.ToUpper()    
        ).ToList();

        var missingHeaders = expectedHeaders.Where(
            expectedHeader =>
                currentHeaders?.Contains(expectedHeader) != true
        ).ToList();

        if (missingHeaders.IsNullOrEmpty())
        {
            return null;
        }

        var errors =  $"Header '{string.Join(", " , missingHeaders)}' tidak ditemukan dalam CSV";
        return errors;
    }
}