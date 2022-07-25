using System.Linq.Expressions;
using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using PersonalFinanceManagement.API.Database.Entities;

namespace PersonalFinanceManagement.API.Database
{
    public class TransactionDbContext : DbContext
    {
        public TransactionDbContext()
        {
        }
        public TransactionDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<TransactionEntity> Transaction { get; set; }
        public DbSet<CategoryEntity> Category { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }

}