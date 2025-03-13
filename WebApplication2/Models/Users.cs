﻿using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Users : IdentityUser
    {
        public string FullName { get; set; }
        public string Role { get; set; } 
    }
}
