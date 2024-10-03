using ExpertOffers.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Client 
    {
        public Guid ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Guid UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<SavedItem> SavedItems { get; set; }
    }
}
