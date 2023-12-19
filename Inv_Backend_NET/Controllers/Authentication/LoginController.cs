using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.DTO.Authentication;
using Inventory_Backend_NET.Models;
using Microsoft.AspNetCore.Mvc;
using Inventory_Backend_NET.Service;

namespace Inventory_Backend_NET.Controllers.Authentication
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly MyDbContext _db;
        private readonly IJwtTokenService _jwtTokenService;
        public LoginController(
            MyDbContext db,
            IJwtTokenService jwtTokenService
        ){
            _db = db;
            _jwtTokenService = jwtTokenService;
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
                    token = _jwtTokenService.GenerateNewToken(currentUser),
                    user = UserDto.From(currentUser)
                });
            }
        }

        
    }
    
    public class LoginBodyDto
    {
        [JsonPropertyName("username")] 
        public string Username { get; set; } = null!;

        [JsonPropertyName("password")] 
        public string Password { get; set; } = null!;
    }
}

