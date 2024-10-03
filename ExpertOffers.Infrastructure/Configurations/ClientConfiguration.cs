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
    public class ClientConfiguration : IEntityTypeConfiguration<Client>
    {
        public void Configure(EntityTypeBuilder<Client> builder)
        {
            builder.HasKey(x => x.ClientID);
            builder.Property(x => x.ClientID).ValueGeneratedOnAdd();

            builder.Property(x => x.FirstName).HasMaxLength(50)
                .IsRequired();

            builder.Property(x => x.LastName)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(x=>x.User)
                .WithOne(x => x.Client)
                .HasForeignKey<Client>(x => x.UserID)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable("Clients");
        }
    }
}
