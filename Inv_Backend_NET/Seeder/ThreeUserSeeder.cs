using Inventory_Backend_NET.Database;
using Inventory_Backend_NET.Database.Models;

namespace Inventory_Backend_NET.Seeder;

/// <summary>
/// Seeder yang akan mengisi database dengan 1 orang admin dan 2 orang user biasa
/// </summary>
public class ThreeUserSeeder
{
    private readonly MyDbContext _db;
    public ThreeUserSeeder(MyDbContext db)
    {
        _db = db;
    }
    public List<User> Run()
    {
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
        _db.Users.AddRange(users);
        _db.SaveChanges();
        return users;
    }
}