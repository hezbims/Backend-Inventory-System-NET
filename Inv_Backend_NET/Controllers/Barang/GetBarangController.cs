using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Barang
{
    [Route("api/[controller]")]
    [ApiController]
    public class GetBarangController : ControllerBase
    {
        private readonly MyDbContext db;
        public GetBarangController(MyDbContext db)
        {
            this.db = db;
        }


    }
}
