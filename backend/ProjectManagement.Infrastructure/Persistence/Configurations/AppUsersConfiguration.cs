using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProjectManagement.Core.Models;

namespace ProjectManagement.Infrastructure.Persistence.Configurations;

public class AppUsersConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.HasKey(a => a.Id);


        //builder
        //    .HasMany(a => a.Projects)
        //    .WithMany(p => p.Users);

        //builder
        //    .HasMany(u => u.Projects)
        //    .WithOne(p => p.Owner)
        //    .HasForeignKey(p => p.Id);
    }
}
