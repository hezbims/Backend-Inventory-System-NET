using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.TestConfiguration.Seeder;

public static class SeedThreeUserExtension
{
    /// <summary>
    /// Mengseed tiga user, yaitu admin, non-admin1, dan non-admin2
    /// </summary>
    /// <param name="db"></param>
    /// <returns>
    /// Mereturn admin, non-admin1, dan non-admin2 (Tanpa id)
    /// </returns>
    public static (User, User, User) SeedThreeUser(this MyDbContext db)
    {
        var admin = new User(username: "admin", password: "123", isAdmin: true);
        var user1 = new User(username: "user1", password: "123", isAdmin: false);
        var user2 = new User(username: "user2", password: "123", isAdmin: false);
        db.Users.AddRange([admin , user1 , user2]);
        db.SaveChanges();

        admin = db.Users.Single(user => user.IsAdmin);
        var nonAdmins = db.Users.Where(user => !user.IsAdmin).ToArray();
        
        return (admin, nonAdmins[0], nonAdmins[1]);
    }
}