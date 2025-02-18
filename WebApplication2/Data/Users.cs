﻿using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class Users
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Fullname { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
