using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Models;

namespace Inventory_Backend.Tests.TestConfiguration.Seeder;

public static class SeedThreeUserExtension
{
    public static (User, User, User) SeedThreeUser(this MyDbContext db)
    {
        var admin = new User(username: "admin", password: "123", isAdmin: true);
        var user1 = new User(username: "user1", password: "123", isAdmin: false);
        var user2 = new User(username: "user2", password: "123", isAdmin: false);
        db.Users.AddRange([admin , user1 , user2]);
        db.SaveChanges();
        
        return (admin, user1, user2);
    }
}