using GymManagementSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GymManagementSystem.DAL.Configurations;

public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.CategoryName)
            .IsRequired()
            .HasMaxLength(50);

        builder.HasQueryFilter(c => !c.IsDeleted);

        builder.HasData(
            new Category { Id = 1, CategoryName = "Cardio" },
            new Category { Id = 2, CategoryName = "Strength" },
            new Category { Id = 3, CategoryName = "Yoga" },
            new Category { Id = 4, CategoryName = "Boxing" },
            new Category { Id = 5, CategoryName = "CrossFit" }
        );
    }
}
