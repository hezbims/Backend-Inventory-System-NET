using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend.Tests.Seeder;

public class UserSeeder(
    MyDbContext dbContext) : IMySeeder
{
    public User Run(
        bool isAdmin,
        string username,
        string password = "this_is_a_password")
    {
        User newUser = new User(
            username: username,
            password: password,
            isAdmin: isAdmin);
        dbContext.Users.Add(newUser);
        dbContext.SaveChanges();
        
        return newUser;
    }

    public User CreateAdmin()
    {
        User newUser = new User(
            username: "some_Admin123",
            password: "1234578A@!",
            isAdmin: true);
        dbContext.Users.Add(newUser);
        dbContext.SaveChanges();
        
        return newUser;
    }
}