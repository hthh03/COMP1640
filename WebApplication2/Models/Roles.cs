namespace WebApplication2.Models
{
    public class Roles
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<UserRoles> UserRoles { get; set; }
    }
}
