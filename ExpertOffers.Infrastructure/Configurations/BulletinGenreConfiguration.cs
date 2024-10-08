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
    public class BulletinGenreConfiguration : IEntityTypeConfiguration<BulletinGenre>
    {
        public void Configure(EntityTypeBuilder<BulletinGenre> builder)
        {
            builder.HasKey(x => x.GenreID);
            builder.Property(x => x.GenreID)
                .ValueGeneratedNever();
            builder.Property(x => x.GenreName)
                .IsRequired()
                .HasMaxLength(50);
            
            builder.ToTable("BulletinGenres");
        }
    }
}
