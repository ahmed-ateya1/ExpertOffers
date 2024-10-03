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
    public class FavoriteConfiguration : IEntityTypeConfiguration<Favorite>
    {
        public void Configure(EntityTypeBuilder<Favorite> builder)
        {
            builder.HasKey(e => e.FavoriteID);

            builder.Property(e => e.FavoriteID)
                .ValueGeneratedNever();

            builder.HasOne(x=>x.Client)
                .WithMany(x=>x.Favorites)
                .HasForeignKey(x=>x.ClientID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Favorites)
                .HasForeignKey(x => x.ClientID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("Favorites");
        }
    }
}
