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
    public class GenreCouponConfiguration : IEntityTypeConfiguration<GenreCoupon>
    {
        public void Configure(EntityTypeBuilder<GenreCoupon> builder)
        {
            builder.HasKey(x=>x.GenreID);
            builder.Property(x => x.GenreID)
                .ValueGeneratedNever();
           
            builder.ToTable("GenreCoupons");
        }
    }
}
