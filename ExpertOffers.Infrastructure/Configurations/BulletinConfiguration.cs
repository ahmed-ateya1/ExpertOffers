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
    public class BulletinConfiguration : IEntityTypeConfiguration<Bulletin>
    {
        public void Configure(EntityTypeBuilder<Bulletin> builder)
        {
            builder.HasKey(x => x.BulletinID);

            builder.Property(x=>x.BulletinID)
                .ValueGeneratedNever();

            builder.Property(x => x.BulletinTitle)
                .IsRequired()
                .HasMaxLength(100);


            builder.Property(x => x.BulletinPictureUrl)
                .IsRequired();

            builder.Property(x => x.BulletinPdfUrl)
                .IsRequired();

            builder.Property(x => x.StartDate)
                .IsRequired();
            builder.Property(x => x.EndDate)
                .IsRequired();

            builder.HasOne(x => x.Company)
                .WithMany(x => x.Bulletins)
                .HasForeignKey(x => x.CompanyID)
                .OnDelete(DeleteBehavior.Cascade);
            builder.HasOne(x => x.Genre)
                .WithMany(x => x.Bulletins)
                .HasForeignKey(x => x.GenreID)
                .OnDelete(DeleteBehavior.Restrict);

            builder.ToTable("Bulletins");
        }
    }
}
