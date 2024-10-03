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
    public class IndustrialConfiguration : IEntityTypeConfiguration<Industrial>
    {
        public void Configure(EntityTypeBuilder<Industrial> builder)
        {
            builder.HasKey(e => e.IndustrialID);

            builder.Property(e => e.IndustrialID)
                .ValueGeneratedNever();

            builder.Property(e => e.IndustrialName)
                .IsRequired()
                .HasMaxLength(50);

            builder.ToTable("Industrials");
        }
    }
}
