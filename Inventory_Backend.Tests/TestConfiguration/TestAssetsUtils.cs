using Microsoft.AspNetCore.Http;

namespace Inventory_Backend.Tests.TestConfiguration;

// Utilities untuk ngambil file dari assets
public class TestAssetsUtils
{
    public static IFormFile GetFormFile(string filename , string jsonKey)
    {
        var csvFile = File.OpenRead($"./TestAssets/{filename}");
        var formFile = new FormFile(
            baseStream: csvFile,
            baseStreamOffset: 0,
            length: csvFile.Length,
            name: jsonKey,
            fileName: csvFile.Name
        );
        return formFile;
    }
}