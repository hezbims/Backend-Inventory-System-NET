﻿using Inventory_Backend_NET.Database.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Inventory_Backend_NET.Database.Models
{
    [EntityTypeConfiguration(typeof(UserConfiguration))]
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Password { get; set; } = null!;
        public bool IsAdmin { get; set; }

        private User(){}

        public User(string username, string password, bool isAdmin , int? id = null)
        {
            Username = username;
            Password = password;
            IsAdmin = isAdmin;
            Id = id ?? default;
        }
        
    }
}
