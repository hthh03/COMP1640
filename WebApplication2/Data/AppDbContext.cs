using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using WebApplication2.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Students> Students { get; set; }
    public DbSet<Blogs> Blogs { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<Files> Files { get; set; }
    public DbSet<Meetings> Meetings { get; set; }
    public DbSet<Messages> Messages { get; set; }
    public DbSet<Users> Users { get; set; }
    public DbSet<UserRoles> UserRoles { get; set; }
    public DbSet<Teachers> Teachers { get; set; }
    public DbSet<Staffs> Staffs { get; set; }
    public DbSet<Roles> Roles { get; set; }
    public DbSet<Recordings> Recordings { get; set; }
    public DbSet<Posts> Posts { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseNpgsql(configuration.GetConnectionString("WebApiDatabase"));
        }
    }
}
