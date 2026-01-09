using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Logic.Extension;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Filters;

namespace Inventory_Backend_NET.Fitur.Autentikasi.GetCurrentUser;

[Route("api/get-current-user")]
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
    [ProducesResponseType(typeof(GetCurrentUserSucceedResponse), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(GetCurrentUserOkExample))]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<GetCurrentUserSucceedResponse> GetCurrentUser()
    {
        var user = _db.GetCurrentUserFrom(_httpContextAccessor);
        if (user is null)
            return Unauthorized();
            
        var userDto = GetUserDto.From(user);
        
        return new GetCurrentUserSucceedResponse(
            Token: _jwtTokenService.GenerateNewToken(user),
            User: userDto);
    }
}

public record GetCurrentUserSucceedResponse(
    [property: JsonPropertyName("token")]
    [UsedImplicitly]
    string Token,
 
    [property: JsonPropertyName("user")]
    [UsedImplicitly]
    GetUserDto User
);

file class GetCurrentUserOkExample : IExamplesProvider<GetCurrentUserSucceedResponse>
{
    public GetCurrentUserSucceedResponse GetExamples()
    {
        return new GetCurrentUserSucceedResponse(
            Token:
            "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3NjgyNzE0MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTE1NCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzAwMCJ9.H46pIQejekT9s-GuXsEauIqGjK2zu2vitYyWAhLRu8Y",
            User: new GetUserDto(id: 1, username: "admin", isAdmin: true));
    }
}