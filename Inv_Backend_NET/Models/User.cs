namespace Inventory_Backend_NET.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsAdmin { get; set; }

        public ICollection<Pengajuan> Pengajuans { get; set; } = new List<Pengajuan>();
    }
}
