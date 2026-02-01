using AuthApp.Application.Interfaces;
using AuthApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthApp.Infrastructure.Data;

public class AppDbContext : DbContext, IApplicationDbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Komfigurasi Fluent API untuk tabel User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.HasIndex(e => e.Email).IsUnique();

            entity.Property(e => e.FullName).HasMaxLength(100).IsRequired();
            entity.Property(e => e.Gender).HasMaxLength(1);
            entity.Property(e => e.Role).HasDefaultValue("User");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
        });
    }
}
