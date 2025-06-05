using Microsoft.EntityFrameworkCore;

namespace BHDTest.Models
{
    public class BHDPruebaContext:DbContext
    {
        public BHDPruebaContext(DbContextOptions<BHDPruebaContext> options)
            : base(options)
        { }
        public DbSet<User> Users { get; set; }
        public DbSet<Phone> Phones { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
