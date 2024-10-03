using ExpertOffers.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Infrastructure.Configurations
{
    public class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> builder)
        {
            builder.HasKey(e => e.CityID);
            builder.Property(x=>x.CityID).ValueGeneratedNever();

            builder.Property(e => e.CityName)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasOne(x => x.Country)
                .WithMany(x => x.Cities)
                .HasForeignKey(x => x.CountryID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(x=>x.Users)
                .WithOne(x => x.City)
                .HasForeignKey(x => x.CityID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("Cities");
        }
    }
}
