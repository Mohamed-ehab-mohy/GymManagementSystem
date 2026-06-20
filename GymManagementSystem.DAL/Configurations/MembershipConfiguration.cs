using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations;

public class MembershipConfiguration : IEntityTypeConfiguration<Membership>
{
    public void Configure(EntityTypeBuilder<Membership> builder)
    {
        builder.HasKey(m => m.Id);

        builder.HasOne(m => m.Member)
            .WithMany(u => u.Memberships)
            .HasForeignKey(m => m.MemberId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.Plan)
            .WithMany()
            .HasForeignKey(m => m.PlanId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(m => new { m.MemberId, m.PlanId }).IsUnique();

        builder.Property(m => m.ReminderDaysSent)
            .HasDefaultValue(0);

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
