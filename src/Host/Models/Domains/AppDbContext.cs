using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;

namespace Host.Models.Domains {
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int> {
        public AppDbContext(DbContextOptions<AppDbContext> options)
           : base(options) {
        }

        protected override void OnModelCreating(ModelBuilder builder) {
            base.OnModelCreating(builder);

            builder.Entity<AppUser>(b => {
                // Shadow properties
                b.Property<DateTime>("Created").HasDefaultValueSql("getdate()");
                b.ToTable("Users");
            });

            builder.Entity<IdentityRole<int>>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<int>>().ToTable("UserClaims");
            builder.Entity<IdentityRoleClaim<int>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserRole<int>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<int>>().ToTable("UserLogins");
            builder.Entity<IdentityUserToken<int>>().ToTable("UserTokens");
        }
    }
}
