﻿using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Users : IdentityUser
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Phone   { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
