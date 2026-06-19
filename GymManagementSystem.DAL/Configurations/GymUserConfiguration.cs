using GymManagementSystem.Domain;
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
            .HasMaxLength(11);

        builder.Property(u => u.DateOfBirth)
            .IsRequired()
            .HasColumnType("date");

        builder.Property(u => u.Gender)
            .IsRequired()
            .HasMaxLength(10);

        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.PhoneNumber).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_GymUser_Email", "Email LIKE '%_@__%.__%'");
            t.HasCheckConstraint("CK_GymUser_Phone", "PhoneNumber LIKE '010[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR PhoneNumber LIKE '011[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR PhoneNumber LIKE '012[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]' OR PhoneNumber LIKE '015[0-9][0-9][0-9][0-9][0-9][0-9][0-9][0-9]'");
        });

        builder.HasDiscriminator<string>("UserType")
            .HasValue<Member>("Member")
            .HasValue<Trainer>("Trainer");

        builder.OwnsOne(u => u.Address, a =>
        {
            a.Property(p => p.Street).HasMaxLength(100);
            a.Property(p => p.City).HasMaxLength(50);
            a.Property(p => p.State).HasMaxLength(50);
            a.Property(p => p.ZipCode).HasMaxLength(20);
        });

        builder.HasQueryFilter(u => !u.IsDeleted);
    }
}
