using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class SavedItem
    {
        public Guid SavedItemID { get; set; }
        public Guid ClientID { get; set; }
        public virtual Client Client { get; set; }

        public Guid? OfferId { get; set; }
        public Offer Offer { get; set; }

        public Guid? CouponId { get; set; }
        public Coupon Coupon { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.UtcNow;

    }
}
