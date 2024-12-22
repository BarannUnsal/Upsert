using Microsoft.EntityFrameworkCore;
using Upsert.Entities;

namespace Upsert.Context
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Name = "Seattle", ModifiedDate = DateTime.Now.AddDays(-1), Age = 19, CreateDate = DateTime.Now.AddYears(-1) },
                new User { Id = 2, Name = "Vancouver", ModifiedDate = DateTime.Now.AddHours(-12), Age = 19, CreateDate = DateTime.Now.AddYears(-1) },
                new User { Id = 3, Name = "Mexico City", ModifiedDate = DateTime.Now.AddDays(-5), Age = 19, CreateDate = DateTime.Now.AddDays(-10) },
                new User { Id = 4, Name = "Puebla", ModifiedDate = DateTime.Now.AddDays(-16), Age = 19, CreateDate = DateTime.Now.AddDays(-16) });

            base.OnModelCreating(modelBuilder);
        }

        public DbSet<User> Users { get; set; }
    }
}
