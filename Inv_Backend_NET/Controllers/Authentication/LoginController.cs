using Inv_Backend_NET.Models;
using Inventory_Backend_NET.DTO.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Inventory_Backend_NET.Controllers.Authentication
{
    [Route("api/login")]
    public class LoginController : Controller
    {
        private readonly MyDbContext db;
        private readonly IConfiguration config;
        public LoginController(
            MyDbContext db,
            IConfiguration config
        ){
            this.db = db;
            this.config = config;
        }

        [HttpPost]
        public IActionResult Login([FromBody] LoginDTO user)
        {
            var currentUser = db.Users.FirstOrDefault( 
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
                    token = generateToken(currentUser)
                });
            }
        }

        private string generateToken(User user)
        {
            var securityKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    config["Jwt:Key"]!
                )
            );
            var credentials = new SigningCredentials(
                securityKey,
                SecurityAlgorithms.HmacSha256
            );

            var claims = new[]
            {
                new Claim(type: ClaimTypes.NameIdentifier , value: user.Username),
                new Claim(type: ClaimTypes.Role , value: user.IsAdmin.ToString())
            };

            var token = new JwtSecurityToken(
                issuer: config["Jwt:Issuer"],
                audience: config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddDays(30),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
