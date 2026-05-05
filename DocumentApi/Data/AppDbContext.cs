using DocumentApi.Models;
using Microsoft.EntityFrameworkCore;

namespace DocumentApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
    : base(options)
    {

    }

    public DbSet<Document> Documents => Set<Document>();
    public DbSet<User> Users => Set<User>();
    public DbSet<DocumentAccess> Acseses => Set<DocumentAccess>();
    public DbSet<DocumentVersion> Versions => Set<DocumentVersion>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DocumentAccess>()
            .HasOne(x => x.User)
            .WithMany(y => y.Accesses)
            .HasForeignKey(x => x.UserId);

        modelBuilder.Entity<DocumentAccess>()
            .HasOne(x => x.Document)
            .WithMany(y => y.Accesses)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DocumentVersion>()
            .HasOne(x => x.Document)
            .WithMany(y => y.Versions)
            .HasForeignKey(x => x.DocumentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<DocumentAccess>()
            .HasKey(x => new { x.UserId, x.DocumentId });

        modelBuilder.Entity<DocumentAccess>()
            .HasIndex(x => new { x.UserId, x.DocumentId })
            .IsUnique();
    }
}