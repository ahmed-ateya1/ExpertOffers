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
    public class OfferConfiguration : IEntityTypeConfiguration<Offer>
    {
        public void Configure(EntityTypeBuilder<Offer> builder)
        {
            builder.HasKey(x => x.OfferID);
            builder.Property(x => x.OfferID)
                .ValueGeneratedNever();

            builder.Property(x => x.OfferTitle)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(x => x.OfferDescription)
                .HasMaxLength(500)
                .IsRequired();

            builder.Property(x => x.OfferPrice)
                .IsRequired();

            builder.Property(x => x.OfferURL)
                .IsRequired();

            builder.Property(x => x.OfferPictureURL)
                .IsRequired();

            builder.Property(x => x.OfferDiscount)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.HasOne(x=>x.Company)
                .WithMany(x => x.Offers)
                .HasForeignKey(x => x.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Genre)
                .WithMany(x => x.Offers)
                .HasForeignKey(x => x.GenreID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Offers");
        }
    }
}
