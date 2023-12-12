using System.Globalization;
using System.Transactions;
using CsvHelper;
using CsvHelper.Configuration;

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
}