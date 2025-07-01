using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur.Barang._Cqrs.Query;
using Inventory_Backend_NET.Fitur.Barang._Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Barang
{
    // TODO : Test kalo current stock yang direturn benar
    [Route("api/barang")]
    public class BarangController(GetBarangsQuery getBarangsQuery) : Controller
    {
        private readonly GetBarangsQuery _getBarangsQuery = getBarangsQuery;

        [HttpGet]
        public IActionResult Get(
            [FromQuery] GetBarangsRequestParams requestParams)
        {
            return Ok(_getBarangsQuery.Execute(requestParams));
        }

        [HttpGet("all")]
        public IActionResult Get(
            [FromQuery(Name = "id_kategori")] int? idKategori,
            [FromQuery] string keyword,
            [FromQuery] int page
        )
        {
            try
            {
                return Ok(_getBarangsQuery.Execute(new GetBarangsRequestParams(
                    SearchKeyword: keyword,
                    Page: page,
                    IdKategori: idKategori
                )));
            }
            catch (Exception e)
            {
                return StatusCode(500, new
                {
                    message = e.Message
                });
            }
        }
    }
}
