using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.DTO.Authentication;
using Microsoft.AspNetCore.Mvc;
using Inventory_Backend_NET.Service;

namespace Inventory_Backend_NET.Controllers.Authentication
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly MyDbContext _db;
        private readonly IJwtTokenBuilder _jwtTokenBuilder;
        public LoginController(
            MyDbContext db,
            IJwtTokenBuilder jwtTokenBuilder
        ){
            _db = db;
            _jwtTokenBuilder = jwtTokenBuilder;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginBodyDto user)
        {
            var currentUser = _db.Users.FirstOrDefault( 
                predicate: aUser => 
                    aUser.Username == user.Username &&
                    aUser.Password == user.Password
            );

            if (currentUser == null)
            {
                return BadRequest(new { message = "Username atau password salah" });
            } else
            {
                return Ok(new
                {
                    message = "Sukses",
                    token = _jwtTokenBuilder.GenerateToken(currentUser)
                });
            }
        }

        
    }
}
