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
    public class CompanyConfiguration : IEntityTypeConfiguration<Company>
    {
        public void Configure(EntityTypeBuilder<Company> builder)
        {
            builder.HasKey(x => x.CompanyID);

            builder.Property(x=>x.CompanyID)
                .ValueGeneratedNever();

            builder.Property(x => x.CompanyName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(x => x.CompanyLogoURL)
                .IsRequired();

            builder.HasOne(x => x.User)
                .WithOne(x => x.Company)
                .HasForeignKey<Company>(x=>x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(x => x.Industrial)
                .WithMany(x => x.Companies)
                .HasForeignKey(x => x.IndustrialID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Companies");
        }
    }
}
