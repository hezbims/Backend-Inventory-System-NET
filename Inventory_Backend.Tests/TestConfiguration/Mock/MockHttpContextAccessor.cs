using System.Security.Claims;
using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur._Constants;
using Microsoft.AspNetCore.Http;

namespace Inventory_Backend.Tests.TestConfiguration.Mock;

public class MockHttpContextAccessor : IHttpContextAccessor
{
    public HttpContext? HttpContext { get; set; }

    public MockHttpContextAccessor(User? user)
    {
        HttpContext = user != null ? new DefaultHttpContext
        {
            User = new ClaimsPrincipal(
                new[]
                {
                    new ClaimsIdentity(
                        new []
                        {
                            new Claim(type: ClaimTypes.NameIdentifier , value: user.Username),
                            new Claim(
                                type: ClaimTypes.Role , 
                                value: user.IsAdmin ?  MyConstants.Roles.Admin : MyConstants.Roles.NonAdmin
                            )
                        }
                    )
                }
            )
        } : new DefaultHttpContext();
    }
}