using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Database.Configurations
{
    public class CategoryEntityTypeConfiguration : IEntityTypeConfiguration<CategoryEntity>
    {
        public void Configure(EntityTypeBuilder<CategoryEntity> builder)
        {

            builder.ToTable("categories");
            builder.HasKey(x => x.Code);
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.ParentCode);
        }
    }
}
