using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Inventory_Backend_NET.Constants;
using Inventory_Backend_NET.Models;
using Microsoft.IdentityModel.Tokens;

namespace Inventory_Backend_NET.Service;

public class JwtTokenService : IJwtTokenService
{
    private readonly IConfiguration _config;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateNewToken(User user)
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
            new Claim(
                type: ClaimTypes.Role , 
                value: user.IsAdmin ?  MyConstants.Roles.Admin : MyConstants.Roles.NonAdmin
            )
        };

        var token = new JwtSecurityToken(
            issuer: _config["JwtSettings:Issuer"]!,
            audience: _config["JwtSettings:Audience"]!,
            claims: claims,
            expires: DateTime.Now.AddDays(7),
            signingCredentials: credentials
        );

        return _tokenHandler.WriteToken(token);
    }

    public string GetUsernameFromJwt(string jwt)
    {
        var decodedJwt = _tokenHandler.ReadJwtToken(jwt);
        var usernameClaim = decodedJwt.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier);
        return usernameClaim.Value;
    }
    
}