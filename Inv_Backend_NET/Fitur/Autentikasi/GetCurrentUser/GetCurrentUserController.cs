using System.Security.Claims;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Constants;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Inventory_Backend_NET.Controllers.Authentication;

[Route("api/get-current-user")]
[Authorize(Policy = MyConstants.Policies.AllUsers)]
public class GetCurrentUserController : ControllerBase
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly MyDbContext _db;
    private readonly IJwtTokenService _jwtTokenService;
    public GetCurrentUserController(
        IHttpContextAccessor httpContextAccessor,
        MyDbContext db,
        IJwtTokenService jwtTokenService
    )
    {
        _httpContextAccessor = httpContextAccessor;
        _db = db;
        _jwtTokenService = jwtTokenService;
    }
    [HttpGet]
    public IActionResult GetCurrentUser()
    {
        var user = _db.GetCurrentUserFrom(_httpContextAccessor);
        var userDto = GetUserDto.From(user);
        
        return Ok(new
        {
            token = _jwtTokenService.GenerateNewToken(user),
            user = userDto
        });
    }
}