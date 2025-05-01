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
    
    /// <param name="path">Path relative dari root test project folder</param>
    public static StreamContent GetFileStream(string path)
    {
        string fullPath = Path.Combine(Directory.GetCurrentDirectory(), path);
        FileStream fileStream = File.OpenRead(path: fullPath);
        return new StreamContent(fileStream);
    }

    public static IFormFile GetDuaBarangSamaCsv()
    {
        return GetFormFile("dua_barang_sama_success_overwrite.csv", jsonKey: "csv");
    }
}