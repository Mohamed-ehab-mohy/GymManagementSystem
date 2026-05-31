using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations;

public class HealthRecordConfiguration : IEntityTypeConfiguration<HealthRecord>
{
    public void Configure(EntityTypeBuilder<HealthRecord> builder)
    {
        builder.HasKey(hr => hr.Id);

        builder.HasOne(hr => hr.Member)
            .WithOne(m => m.HealthRecord)
            .HasForeignKey<HealthRecord>(hr => hr.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Property(hr => hr.BloodType).HasMaxLength(10);
        builder.Property(hr => hr.MedicalConditions).HasMaxLength(500);
        builder.Property(hr => hr.Note).HasMaxLength(1000);

        builder.Property(hr => hr.Weight).HasColumnType("decimal(5,2)");
        builder.Property(hr => hr.Height).HasColumnType("decimal(5,2)");

        builder.Property(hr => hr.LastUpdate).HasDefaultValueSql("GETDATE()");

        builder.HasQueryFilter(hr => !hr.IsDeleted);
    }
}
