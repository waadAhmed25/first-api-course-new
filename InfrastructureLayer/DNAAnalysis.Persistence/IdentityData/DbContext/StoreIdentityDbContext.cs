using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using DNAAnalysis.Domain.Entities.IdentityModule;

namespace DNAAnalysis.Persistence.IdentityData.DbContext
{
    public class StoreIdentityDbContext 
        : IdentityDbContext<ApplicationUser>
    {
        public StoreIdentityDbContext(
            DbContextOptions<StoreIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<OtpCode> OtpCodes { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<IdentityRole>().ToTable("Roles");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
             builder.Entity<ApplicationUser>()
             .HasIndex(u => u.Email)
             .IsUnique();

            builder.Entity<Address>().ToTable("Addresses");
            builder.Entity<OtpCode>()
            .HasOne(o => o.User)
            .WithMany(u => u.OtpCodes)
           .HasForeignKey(o => o.UserId)
           .OnDelete(DeleteBehavior.Cascade);

        }
    }
}
