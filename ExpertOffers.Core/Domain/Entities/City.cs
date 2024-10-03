using ExpertOffers.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class City
    {
        public Guid CityID { get; set; }
        public string CityName { get; set; }
        public Guid CountryID { get; set; }
        public virtual Country Country { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }
    }
}
