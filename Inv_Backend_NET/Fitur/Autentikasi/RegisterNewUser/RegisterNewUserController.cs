using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Fitur.Autentikasi.RegisterNewUser;

[Authorize(policy: MyConstants.Policies.AdminOnly)]
[Route("api/register")]
public class RegisterNewUserController : ControllerBase
{
    private readonly MyDbContext _db;

    public RegisterNewUserController(MyDbContext db)
    {
        _db = db;
    }
    
    [HttpPost]
    public IActionResult Index([FromBody] RegisterRequestBody requestBody)
    {
        try
        {
            var usedUsername = _db.Users.FirstOrDefault(
                user => user.Username == requestBody.Username    
            );
            if (usedUsername != null)
                ModelState.AddModelError("username" , "Username sudah digunakan");

            if (!ModelState.IsValid)
            {
                return BadRequest(new
                {
                    errors = ModelState.ToMinimalDictionary()
                });
            }

            var user = new User(
                username: requestBody.Username,
                password: requestBody.Password,
                isAdmin: requestBody.IsAdmin ?? throw new NoNullAllowedException("Kesalahan kodingan")
            );
            _db.Users.Add(user);
            _db.SaveChanges();

            return Ok(new
            {
                message = "Success"
            });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return StatusCode(500, e.Message);
        }
    }
}

public class RegisterRequestBody
{
    [JsonPropertyName("username")]
    [StringLength(maximumLength: 16, MinimumLength = 3, ErrorMessage = "Panjang username harus diantara 3-16")]
    public string Username { get; init; } = null!;

    [JsonPropertyName("password")]
    [StringLength(maximumLength: 16, MinimumLength = 3, ErrorMessage = "Panjang password harus diantara 3-16")]
    public string Password { get; init; } = null!;
    
    [JsonPropertyName("is_admin")]
    [Required(ErrorMessage = "Kesalahan kodingan front-end. field is_admin_tidak ada")]
    public bool? IsAdmin { get; init; }
}