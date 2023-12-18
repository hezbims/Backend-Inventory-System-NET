using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend_NET.Seeder;

public static class UserOnlySeederExtension
{
    public static List<User> SeedUser(
        this IServiceProvider serviceProvider 
    )
    {
        var db = serviceProvider.GetRequiredService<MyDbContext>();
        var users = new List<User>( new []{
                new User(
                    username : "admin",
                    password : "123",
                    isAdmin : true
                ),
                new User(
                    username : "hezbi",
                    password : "123",
                    isAdmin : false
                ),
                new User(
                    username : "hasbi",
                    password : "123",
                    isAdmin : false
                )
            }
        );
        db.Users.AddRange(users);
        db.SaveChanges();
        return users;
    }
}