using System.Text.Json.Serialization;
using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Fitur._Logic.Services;
using Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Filters;

namespace Inventory_Backend_NET.Fitur.Autentikasi.Login
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
        [AllowAnonymous]
        [SwaggerResponse(StatusCodes.Status200OK, "Login Sukses", typeof(LoginSucceedResponse))]
        [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LoginSucceedResponseExample))]
        [SwaggerResponse(StatusCodes.Status400BadRequest, "Login gagal", typeof(LoginFailedResponse))]
        [SwaggerResponseExample(StatusCodes.Status400BadRequest, typeof(LoginFailedResponseExample))]
        public IActionResult Login([FromBody] LoginRequest user)
        {
            var currentUser = _db.Users.FirstOrDefault( 
                predicate: aUser => 
                    aUser.Username == user.Username &&
                    aUser.Password == user.Password
            );

            if (currentUser == null)
            {
                return BadRequest(new LoginFailedResponse(Message: "Username atau password salah" ));
            } else
            {
                return Ok(new LoginSucceedResponse
                (
                    Message: "Sukses",
                    Token: _jwtTokenService.GenerateNewToken(currentUser),
                    User: GetUserDto.From(currentUser)
                ));
            }
        }

        
    }

    internal record LoginSucceedResponse(
        [property: JsonPropertyName("token")][UsedImplicitly] string Token,
        [property: JsonPropertyName("user")][UsedImplicitly] GetUserDto User,
        [property: JsonPropertyName("message")][UsedImplicitly] string Message);

    internal record LoginFailedResponse(
        [property: JsonPropertyName("message")][UsedImplicitly] string Message);
    
    public class LoginRequest
    {
        [JsonPropertyName("username")] 
        public string Username { get; set; } = null!;

        [JsonPropertyName("password")] 
        public string Password { get; set; } = null!;
    }

    file class LoginSucceedResponseExample : IExamplesProvider<LoginSucceedResponse>
    {
        public LoginSucceedResponse GetExamples()
        {
            return new LoginSucceedResponse(
                Token: "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJodHRwOi8vc2NoZW1hcy54bWxzb2FwLm9yZy93cy8yMDA1LzA1L2lkZW50aXR5L2NsYWltcy9uYW1laWRlbnRpZmllciI6ImFkbWluIiwiaHR0cDovL3NjaGVtYXMubWljcm9zb2Z0LmNvbS93cy8yMDA4LzA2L2lkZW50aXR5L2NsYWltcy9yb2xlIjoiQWRtaW4iLCJleHAiOjE3NjgyNzE0MDEsImlzcyI6Imh0dHA6Ly9sb2NhbGhvc3Q6NTE1NCIsImF1ZCI6Imh0dHA6Ly9sb2NhbGhvc3Q6MzAwMCJ9.H46pIQejekT9s-GuXsEauIqGjK2zu2vitYyWAhLRu8Y",
                User: new GetUserDto(id: 1, username: "admin", isAdmin: true),
                Message: "Sukses"
            );
        }
    }

    file class LoginFailedResponseExample : IExamplesProvider<LoginFailedResponse>
    {
        public LoginFailedResponse GetExamples()
        {
            return new LoginFailedResponse(Message: "Username atau password salah");
        }
    }
}

