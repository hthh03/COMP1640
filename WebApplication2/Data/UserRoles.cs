using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class UserRoles
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public Users Users { get; set; }
        public string RoleId { get; set; }
        public Roles Roles { get; set; }

    }
}
