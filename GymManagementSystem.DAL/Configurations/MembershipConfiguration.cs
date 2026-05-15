using GymManagementSystem.DAL.Entities;
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

        // Unique Index to prevent double enrollment in the exact same plan at the same time
        // Note: For a real gym, a user might renew a plan, but usually we handle that differently.
        builder.HasIndex(m => new { m.MemberId, m.PlanId }).IsUnique();

        builder.HasQueryFilter(m => !m.IsDeleted);
    }
}
