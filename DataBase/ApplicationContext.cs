using DataBase.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace DataBase
{
    public class ApplicationContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Advert> Adverts { get; set; }

        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Advert>()
                .Property(adv => adv.DateTime)
                .HasDefaultValueSql("GETDATE()");

            modelBuilder.Entity<Advert>()
                .Property(adv => adv.Rating)
                .HasDefaultValue(0);
        }
    }

    public class ApplicationContextFactory : IDesignTimeDbContextFactory<ApplicationContext>
    {
        public ApplicationContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationContext>();
            optionsBuilder.UseSqlServer("Server=NASTIA\\MSSQLSERVER01;Database=AdvertDb;Trusted_Connection=True;MultipleActiveResultSets=true", b => b.MigrationsAssembly("DataBase"));

            return new ApplicationContext(optionsBuilder.Options);
        }
    }
}
