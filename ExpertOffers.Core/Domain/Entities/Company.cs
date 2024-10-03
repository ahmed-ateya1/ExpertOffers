using ExpertOffers.Core.Domain.IdentityEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Company 
    {
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoURL { get; set; }
        public Guid UserID { get; set; }
        public virtual ApplicationUser User { get; set; }
        public Guid IndustrialID { get; set; }
        public virtual Industrial Industrial { get; set; }
        public virtual ICollection<Branch> Branches { get; set; }
        public virtual ICollection<Favorite> Favorites { get; set; }
        public virtual ICollection<Bulletin> Bulletins { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
