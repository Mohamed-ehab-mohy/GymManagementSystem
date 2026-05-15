using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations;

public class GymUserConfiguration : IEntityTypeConfiguration<GymUser>
{
    public void Configure(EntityTypeBuilder<GymUser> builder)
    {
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FirstName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.LastName)
            .IsRequired()
            .HasMaxLength(50);

        builder.Property(u => u.Email)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(u => u.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);

        // Check Constraint for Email and Phone
        builder.HasCheckConstraint("CK_GymUser_Email", "Email LIKE '%_@__%.__%'");
        builder.HasCheckConstraint("CK_GymUser_Phone", "PhoneNumber NOT LIKE '%[^0-9]%'");

        // TPH Configuration
        builder.HasDiscriminator<string>("UserType")
            .HasValue<Member>("Member")
            .HasValue<Trainer>("Trainer");

        // Owned Type
        builder.OwnsOne(u => u.Address, a =>
        {
            a.Property(p => p.Street).HasMaxLength(100);
            a.Property(p => p.City).HasMaxLength(50);
            a.Property(p => p.State).HasMaxLength(50);
            a.Property(p => p.ZipCode).HasMaxLength(20);
        });

        // Global Query Filter for Soft Delete
        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
