using Gassy.Helpers;
using Microsoft.Extensions.Options;
using Microsoft.EntityFrameworkCore;
using MySql.EntityFrameworkCore.Extensions;

namespace Gassy.Models
{
    public class GassyDbContext : DbContext
    {
       public GassyDbContext(DbContextOptions<GassyDbContext> options) 
            : base(options) 
       {
           
       }


        public DbSet<Listing> Listing { get; set;}
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Listing>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.SiteId).IsRequired();
                entity.Property(e => e.Make);
                entity.Property(e => e.Model);
                entity.Property(e => e.Price);

            });

            // modelBuilder.Entity<Book>(entity =>
            // {
            //     entity.HasKey(e => e.ISBN);
            //     entity.Property(e => e.Title).IsRequired();
            //     entity.HasOne(d => d.Publisher)
            //     .WithMany(p => p.Books);
            // });
        }

    }
}