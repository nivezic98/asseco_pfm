using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Database.Configurations
{
    public class TransactionEntityTypeConfigurations : IEntityTypeConfiguration<TransactionEntity>
    {
        public void Configure(EntityTypeBuilder<TransactionEntity> builder)
        {
            builder.ToTable("transactions");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.BeneficiaryName).HasMaxLength(128);
            builder.Property(x => x.Date).IsRequired();
            builder.Property(x => x.Direction).IsRequired().HasConversion<string>().HasMaxLength(1);
            builder.Property(x => x.Amount).IsRequired();
            builder.Property(x => x.Currency).IsRequired().HasMaxLength(3);
            builder.Property(x => x.Kind).IsRequired().HasConversion<string>();
        }
    }
}
