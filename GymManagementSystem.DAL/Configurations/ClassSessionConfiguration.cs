using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations;

public class ClassSessionConfiguration : IEntityTypeConfiguration<ClassSession>
{
    public void Configure(EntityTypeBuilder<ClassSession> builder)
    {
        builder.HasKey(cs => cs.Id);

        builder.Property(cs => cs.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasOne(cs => cs.Trainer)
            .WithMany(t => t.ClassSessions)
            .HasForeignKey(cs => cs.TrainerId)
            .OnDelete(DeleteBehavior.Restrict);

        // Check Constraint for Capacity
        builder.HasCheckConstraint("CK_ClassSession_Capacity", "Capacity > 0 AND Capacity <= 100");

        builder.HasQueryFilter(cs => !cs.IsDeleted);
    }
}
