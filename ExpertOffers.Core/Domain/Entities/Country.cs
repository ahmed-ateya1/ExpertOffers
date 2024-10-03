using ExpertOffers.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Country
    {
        public Guid CountryID { get; set; }
        public string CountryName { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
        public virtual ICollection<City> Cities { get; set; }
    }
}
