﻿using System.Text.Json.Serialization;

namespace Inventory_Backend_NET.DTO.Authentication
{

    public class LoginDTO
    {
        [JsonPropertyName("username")] 
        public string Username { get; set; } = null!;

        [JsonPropertyName("password")] 
        public string Password { get; set; } = null!;
    }
}
