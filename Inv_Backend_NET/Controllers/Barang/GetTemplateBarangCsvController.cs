using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Barang;

[Route("api/barang/csv-template-download")]
public class GetTemplateBarangCsvController : ControllerBase
{
    private readonly string _csvPath;
    public GetTemplateBarangCsvController(IWebHostEnvironment env)
    {
        _csvPath = Path.Combine(env.ContentRootPath, "Assets/template_input_barang.csv");
    }

    [HttpGet]
    public FileContentResult Index()
    {
        var fileBytes = System.IO.File.ReadAllBytes(_csvPath);

        return new FileContentResult(fileBytes , "application/octet-stream")
        {
            FileDownloadName = "template_input_barang.csv"
        };
    }
}