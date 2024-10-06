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
    public class CouponConfiguration : IEntityTypeConfiguration<Coupon>
    {
        public void Configure(EntityTypeBuilder<Coupon> builder)
        {
            builder.HasKey(x => x.CouponID);

            builder.Property(x => x.CouponCode)
                .IsRequired()
                .HasMaxLength(50);  
            
            builder.Property(x => x.DiscountPercentage)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();

            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.Property(x => x.CouponePictureURL)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(x => x.CouponeURL);

            builder.HasOne(x=>x.Company)
                .WithMany(x=> x.Coupons)
                .HasForeignKey(x => x.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.GenreCoupon)
                .WithMany(x => x.Coupons)
                .HasForeignKey(x => x.GenreID)
                .OnDelete(DeleteBehavior.Restrict);
            builder.ToTable("Coupons");
        }
    }
}
