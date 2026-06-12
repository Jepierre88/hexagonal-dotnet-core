using Domain.Iam;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Adapters.Out.Persistence;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : IdentityDbContext<AppUser>(options)
{
    public DbSet<Scope> Scopes => Set<Scope>();
    public DbSet<RoleScope> RoleScopes => Set<RoleScope>();
    public DbSet<UserScope> UserScopes => Set<UserScope>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Scope>(e =>
        {
            e.HasKey(s => s.Id);
            e.HasIndex(s => s.Name).IsUnique();
            e.Property(s => s.Name).HasMaxLength(200).IsRequired();
            e.Property(s => s.Description).HasMaxLength(500);
        });

        builder.Entity<RoleScope>(e =>
        {
            e.HasKey(rs => new { rs.RoleId, rs.ScopeId });
            e.HasOne(rs => rs.Role)
                .WithMany()
                .HasForeignKey(rs => rs.RoleId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(rs => rs.Scope)
                .WithMany(s => s.RoleScopes)
                .HasForeignKey(rs => rs.ScopeId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        builder.Entity<UserScope>(e =>
        {
            e.HasKey(us => new { us.UserId, us.ScopeId });
            e.HasOne(us => us.User)
                .WithMany(u => u.UserScopes)
                .HasForeignKey(us => us.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(us => us.Scope)
                .WithMany(s => s.UserScopes)
                .HasForeignKey(us => us.ScopeId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}