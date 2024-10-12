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
    public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
    {
        public void Configure(EntityTypeBuilder<Notification> builder)
        {
            builder.HasKey(x => x.NotificationID);

            builder.Property(x => x.NotificationID)
                .ValueGeneratedNever();

            builder.Property(x => x.Message)
                .IsRequired()
                .HasMaxLength(300);

            builder.Property(x => x.CreatedDate)
                .IsRequired();

            builder.Property(x => x.IsRead)
                .IsRequired();

            builder.HasOne(x => x.Coupon)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.CouponId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Bulletin)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.BulletinId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.CompanyID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Offer)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.OfferId)
                .OnDelete(DeleteBehavior.NoAction);

            builder.HasOne(x => x.Client)
                .WithMany(x => x.Notifications)
                .HasForeignKey(x => x.ClientID)
                .OnDelete(DeleteBehavior.NoAction);

            builder.ToTable("Notifications");
        }
    }
}
