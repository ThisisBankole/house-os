using Microsoft.EntityFrameworkCore;
using HouseOs.Models;

namespace HouseOs.Data;

public class ApplicationContext : DbContext
{
    public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
    {
    }

    public DbSet<Grocery> Groceries { get; set; }
    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Grocery>(entity =>
        {
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.Name).HasColumnType("varchar(255)");
            entity.Property(e => e.Quantity).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TotalCost).HasColumnType("decimal(18,2)");
            entity.Property(e => e.UnitCost).HasColumnType("decimal(18,2)");
            
           
            
            entity.Property(e => e.CreatedAt).HasColumnType("timestamp with time zone");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.Property(e => e.Id).UseIdentityByDefaultColumn();
            entity.Property(e => e.FirstName).HasColumnType("text");
            entity.Property(e => e.LastName).HasColumnType("text");
            entity.Property(e => e.Email).HasColumnType("text");
            entity.Property(e => e.Password).HasColumnType("text");
        });
    }
}