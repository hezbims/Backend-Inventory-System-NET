using System.Security.Claims;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.DTO.Authentication;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Authentication;

[Route("api/get-current-user")]
[Authorize(Policy = Policies.AllUsers)]
public class GetCurrentUserController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MyDbContext _db;
    private readonly IJwtTokenBuilder _jwtTokenBuilder;
    public GetCurrentUserController(
        IHttpContextAccessor httpContextAccessor,
        MyDbContext db,
        IJwtTokenBuilder jwtTokenBuilder
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
        _jwtTokenBuilder = jwtTokenBuilder;
    }
    [HttpGet]
    public IActionResult GetCurrentUser()
    {
        var username = _httpContextAccessor.HttpContext!.User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var user = _db.Users.First(user => user.Username == username);
        var userDto = UserDto.From(user);
        Console.WriteLine(userDto.Username);
        return Ok(new
        {
            token = _jwtTokenBuilder.GenerateNewToken(user),
            user = userDto
        });
    }
}