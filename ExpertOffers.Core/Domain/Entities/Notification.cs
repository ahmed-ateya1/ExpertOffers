using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Notification
    {
        public Guid NotificationID { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
        public string? ReferenceURL { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public Guid? ClientID { get; set; }
        public virtual Client Client { get; set; }
        public Guid? CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public Guid? OfferId { get; set; }
        public virtual Offer Offer { get; set; }

        public Guid? CouponId { get; set; }
        public virtual Coupon Coupon { get; set; }

        public Guid? BulletinId { get; set; }
        public virtual Bulletin Bulletin { get; set; }

    }
}
