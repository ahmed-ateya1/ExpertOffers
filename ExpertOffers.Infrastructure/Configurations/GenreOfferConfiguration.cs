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
    public class GenreOfferConfiguration : IEntityTypeConfiguration<GenreOffer>
    {
        public void Configure(EntityTypeBuilder<GenreOffer> builder)
        {
            builder.HasKey(e => e.GenreID);

            builder.Property(e => e.GenreID)
                .ValueGeneratedNever();

            builder.Property(e => e.GenreName)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.GenreImgURL)
                .IsRequired();

            builder.ToTable("GenreOffers");
        }
    }
}
