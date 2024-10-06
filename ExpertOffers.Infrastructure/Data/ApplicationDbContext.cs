using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Infrastructure.Configurations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Infrastructure.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser , ApplicationRole ,Guid>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Offer> Offers { get; set; }
        public DbSet<GenreOffer> GenreOffers { get; set; }
        public DbSet<SavedItem> SavedItems { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Industrial> Industrials { get; set; }
        public DbSet<Bulletin> Bulletins { get; set; }
        public DbSet<Favorite> Favorites { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Coupon> Coupons { get; set; }
        public DbSet<GenreCoupon> GenreCoupons { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfigurationsFromAssembly(typeof(ClientConfiguration).Assembly);
            base.OnModelCreating(builder);
        }
    }
}
