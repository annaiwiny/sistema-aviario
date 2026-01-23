using FarmSystemProject.Models.Farm;
using FarmSystemProject.Models.HealthMonitoring;
using FarmSystemProject.Models.NutritionalControl;
using FarmSystemProject.Models.ProductiveMonitoring;
using FarmSystemProject.Models.Users;
using Microsoft.EntityFrameworkCore;

namespace FarmSystemProject.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
        
    }
    public DbSet<User> Users { get; set; }
    public DbSet<Farm> Farms { get; set; }
    public DbSet<Lot> Lots { get; set; }
    public DbSet<Race> Races { get; set; }
    public DbSet<Mortality> Mortalities { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Feeding> Feedings { get; set; }
    public DbSet<CollectEgg> CollectEggs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Farm>().HasIndex(f => f.OwnerId).IsUnique();
        modelBuilder.Entity<Feed>().Property(f => f.BagValue).HasPrecision(18, 2);
    }
}
