using FarmSystemProject.Models.Farms;
using FarmSystemProject.Models.Lots;
using FarmSystemProject.Models.HealthMonitoring;
using FarmSystemProject.Models.NutritionalControl;
using FarmSystemProject.Models.ProductiveMonitoring;
using FarmSystemProject.Models.SalesRecord;
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
    public DbSet<Lineage> Lineages { get; set; }
    public DbSet<Mortality> Mortalities { get; set; }
    public DbSet<Vaccination> Vaccinations { get; set; }
    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Feeding> Feedings { get; set; }
    public DbSet<EggProduction> EggProductions { get; set; }
    public DbSet<Sale> Sales { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(u => u.Cpf).IsUnique();

        modelBuilder.Entity<Farm>().HasIndex(f => f.OwnerId).IsUnique();

        modelBuilder.Entity<Lot>()
            .HasMany(l => l.Lineages)
            .WithOne(i => i.Lot)
            .HasForeignKey(i => i.LotId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Feed>().Property(f => f.BagValue).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(s => s.UnitValue).HasPrecision(18, 2);
        modelBuilder.Entity<Sale>().Property(s => s.TotalValue).HasPrecision(18, 2);
    }
}