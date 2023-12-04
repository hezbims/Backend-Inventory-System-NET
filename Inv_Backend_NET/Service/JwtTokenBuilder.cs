using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory_Backend_NET.Models;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Service;

public class JwtTokenBuilder : IJwtTokenBuilder
{
    readonly IConfiguration _config;

    public JwtTokenBuilder(IConfiguration config)
    {
        _config = config;
    }

    public string GenerateToken(User user)
    {
        var securityKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(
                _config["JwtSettings:Key"]!
            )
        );
        var credentials = new SigningCredentials(
            securityKey,
            SecurityAlgorithms.HmacSha256
        );

        var claims = new[]
        {
            new Claim(type: ClaimTypes.NameIdentifier , value: user.Username),
            new Claim(type: ClaimTypes.Role , value: user.IsAdmin ? "Admin" : "Non-Admin")
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"]!,
            audience: _config["JwtSettings:Audience"]!,
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}