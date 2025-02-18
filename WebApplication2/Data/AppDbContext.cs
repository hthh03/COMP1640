using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication2.Data;


public class AppDbContext : DbContext
{
    protected readonly IConfiguration Configuration;

    public AppDbContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        // connect to postgres with connection string from app settings
        options.UseNpgsql(Configuration.GetConnectionString("WebApiDatabase"));
    }
    public DbSet<Students> Students {  get; set; }
    public DbSet<Blogs> Blogs { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<Files> Files { get; set; }
    public DbSet<Meetings> Meetings { get; set; }
    public DbSet<Messages> Messages { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    public DbSet<Teachers> Teachers { get; set; }
    //public DbSet<UpdateProfile> UpdateProfiles { get; set; }
    public DbSet<Staffs> Staffs { get; set; }
    public DbSet<Roles> Roles { get; set; }
    //public Recordings Recordings { get; set; }  
    //public Posts Posts { get; set; }

}