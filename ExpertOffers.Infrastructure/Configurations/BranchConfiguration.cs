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
    public class BranchConfiguration : IEntityTypeConfiguration<Branch>
    {
        public void Configure(EntityTypeBuilder<Branch> builder)
        {
            builder.HasKey(e => e.BranchID);

            builder.Property(e => e.BranchID)
                .ValueGeneratedNever();

            builder.Property(e => e.BranchName)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(e => e.PhoneNumber)
                .HasMaxLength(20)
                .IsRequired(false);

            builder.Property(e => e.Location)
                .HasMaxLength(200)
                .IsRequired();


            builder.HasOne(x => x.Company)
                .WithMany(x => x.Branches)
                .HasForeignKey(x => x.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Branches");
        }
    }
}
