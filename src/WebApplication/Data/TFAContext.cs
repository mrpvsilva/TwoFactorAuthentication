using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApplication.Entities;

namespace WebApplication.Data
{
    public class TFAContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public TFAContext(DbContextOptions<TFAContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(UserBuilder);

            base.OnModelCreating(modelBuilder);
        }


        private void UserBuilder(EntityTypeBuilder<User> builder)
        {
            builder
                .HasKey(x => x.Id);

            builder
                .HasIndex(x => x.Email)
                .IsUnique();
        }
    }
}
