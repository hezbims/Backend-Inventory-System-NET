using Inventory_Backend_NET.Database.Models;
using Inventory_Backend.Tests.TestConfiguration.Fixture;
using Xunit.Abstractions;

namespace Inventory_Backend.Tests.TestConfiguration;

public class BaseIntegrationTest : BaseEmptyIntegrationTest
{
    public BaseIntegrationTest(TestWebAppFactory webApp, ITestOutputHelper output) : base(webApp, output)
    {
        using var db = webApp.GetDbContext();
        List<User> users =
        [
            new User(username: "admin", password: "admin123", isAdmin: true),
            new User(username: "non_admin", password: "non-admin123", isAdmin: false)
        ];
        db.AddRange(users);
        db.SaveChanges();
    }
}