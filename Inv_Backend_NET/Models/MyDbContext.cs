using Microsoft.EntityFrameworkCore;

namespace Inv_Backend_NET.Models
{
    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options): base(options)
        { }

        public DbSet<User> Users { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(user => user.Username)
                .IsUnique();
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    Password = "123",
                    IsAdmin = true,
                },
                new User
                {
                    Id = 2,
                    Username = "hezbi",
                    Password = "123",
                    IsAdmin = false,
                }
            );
        }
    }
}
