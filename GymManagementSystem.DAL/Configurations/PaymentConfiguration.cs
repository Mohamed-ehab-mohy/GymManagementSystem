using GymManagementSystem.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.ToTable("Payments");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(p => p.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(p => p.Status)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(p => p.PaymobOrderId)
            .HasMaxLength(100);

        builder.Property(p => p.PaymobTransactionId)
            .HasMaxLength(100);

        builder.HasOne(p => p.Member)
            .WithMany()
            .HasForeignKey(p => p.MemberId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(p => p.Membership)
            .WithMany()
            .HasForeignKey(p => p.MembershipId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
