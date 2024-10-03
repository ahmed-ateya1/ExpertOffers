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
    public class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> builder)
        {
            builder.HasKey(e => e.CountryID);

            builder.Property(e => e.CountryID)
                .ValueGeneratedNever();

            builder.Property(e => e.CountryName)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasMany(x=>x.Users)
                .WithOne(x => x.Country)
                .HasForeignKey(x => x.CountryID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("Countries");
        }
    }
}
