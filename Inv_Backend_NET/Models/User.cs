using Microsoft.EntityFrameworkCore;

namespace Inv_Backend_NET.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsAdmin { get; set; } 
    }
}
