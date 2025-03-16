using Inventory_Backend_NET.Database.Models;
using Inventory_Backend_NET.Fitur.Autentikasi._Dto.Response;

namespace Inventory_Backend_NET.Fitur.Autentikasi._Mapper;

public static class UserMapper
{
    public static GetUserDto ToGetUserResponseDto(this User u)
    {
        return new GetUserDto(
            username: u.Username,
            isAdmin: u.IsAdmin,
            id: u.Id);
    }
}