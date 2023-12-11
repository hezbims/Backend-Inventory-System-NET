using System.Security.Claims;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.DTO.Authentication;
using Inventory_Backend_NET.Models;
using Inventory_Backend_NET.Service;
using Inventory_Backend_NET.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Authentication;

[Route("api/get-current-user")]
[Authorize(Policy = MyConstants.Policies.AllUsers)]
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
        var user = _db.GetCurrentUserFrom(_httpContextAccessor);
        var userDto = UserDto.From(user);
        
        return Ok(new
        {
            token = _jwtTokenBuilder.GenerateNewToken(user),
            user = userDto
        });
    }
}