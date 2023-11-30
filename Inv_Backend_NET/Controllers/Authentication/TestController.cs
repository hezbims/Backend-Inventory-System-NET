using Inv_Backend_NET.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Validations;

namespace Inventory_Backend_NET.Controllers.Authentication
{
    [Route("api/tes")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly MyDbContext db;
        public TestController(MyDbContext db)
        {
            this.db = db;
        }

        [HttpGet]
        public IActionResult tes()
        {
            var users = db.Users.ToList();

            return Ok(users);
        }
    }
}
