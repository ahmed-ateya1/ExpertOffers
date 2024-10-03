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
    public class SavedItemConfiguration : IEntityTypeConfiguration<SavedItem>
    {
        public void Configure(EntityTypeBuilder<SavedItem> builder)
        {
            builder.HasKey(x=>x.SavedItemID);
            builder.Property(x => x.SavedItemID).ValueGeneratedNever();

            builder.HasOne(x=>x.Client)
                .WithMany(x => x.SavedItems)
                .HasForeignKey(x => x.ClientID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Offer)
                .WithMany(x => x.SavedItems)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Coupon)
                .WithMany(x => x.SavedItems)
                .HasForeignKey(x => x.CouponId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Bulletin)
                .WithMany(x => x.SavedItems)
                .HasForeignKey(x => x.BulletinID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("SavedItems");
        }
    }
}
