using ExpertOffers.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Favorite
    {
        public Guid FavoriteID { get; set; }
        public Guid CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public Guid ClientID { get; set; }
        public virtual Client Client { get; set; }
        public DateTime DateAdded { get; set; } = DateTime.UtcNow;
    }
}
