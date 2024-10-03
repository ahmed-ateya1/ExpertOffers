using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.AuthenticationDTO;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.IdentityEntities
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public Guid? ClientID { get; set; }
        public virtual Client Client { get; set; }

        public Guid? ComapnyID { get; set; }
        public virtual Company Company { get; set; }

        public Guid? CountryID { get; set; }
        public virtual Country Country { get; set; }

        public Guid? CityID { get; set; }
        public virtual City City { get; set; }

        public ICollection<RefreshToken>? RefreshTokens { get; set; }
    }
}
