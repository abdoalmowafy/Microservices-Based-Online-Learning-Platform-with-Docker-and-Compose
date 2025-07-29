using auth.Models;
using Microsoft.EntityFrameworkCore;

namespace auth.Data
{
    public class AuthDbContext: DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options)
        {
        }

         public DbSet<AuthUser> AuthUsers { get; set; }
         public DbSet<AuthRefreshToken> AuthRefreshTokens { get; set; }
         public DbSet<AuthEmailVerficationToken> AuthEmailVerficationTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<AuthUser>()
                .Property(u => u.Role)
                .HasConversion<string>();

            modelBuilder.Entity<AuthUser>()
                .HasIndex(u => u.Email)
                .IsUnique();

            modelBuilder.Entity<AuthRefreshToken>()
                .HasIndex(rt => rt.RefreshToken)
                .IsUnique();
        }
    }
}
