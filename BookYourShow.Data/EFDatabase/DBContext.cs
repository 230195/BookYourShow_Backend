using System;
using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BookYourShow.Data.EFDatabase {
    public class BYSDBContext : IdentityDbContext<ApplicationUser> {
        public BYSDBContext (DbContextOptions<BYSDBContext> options) : base (options) { }
        protected override void OnModelCreating (ModelBuilder modelBuilder) {
            base.OnModelCreating (modelBuilder);
            var allEntities = modelBuilder.Model.GetEntityTypes ();

            foreach (var entity in allEntities) {
                entity.AddProperty ("CreatedDate", typeof (DateTime));
                entity.AddProperty ("UpdatedDate", typeof (DateTime));
            }

            foreach (var foreignKey in modelBuilder.Model.GetEntityTypes ().SelectMany (e => e.GetForeignKeys ())) {
                foreignKey.DeleteBehavior = DeleteBehavior.Restrict;
            }
        }

        public override int SaveChanges () {
            var entries = ChangeTracker
                .Entries ()
                .Where (e =>
                    e.State == EntityState.Added ||
                    e.State == EntityState.Modified);

            foreach (var entityEntry in entries) {
                if (entityEntry.State == EntityState.Added) {
                    entityEntry.Property ("CreatedDate").CurrentValue = DateTime.Now;
                } else {
                    entityEntry.Property ("UpdatedDate").CurrentValue = DateTime.Now;
                }
            }

            return base.SaveChanges ();
        }
        public DbSet<Theater> Theater { get; set; }
        public DbSet<City> City { get; set; }
        public DbSet<Movie> Movie { get; set; }
    }
}