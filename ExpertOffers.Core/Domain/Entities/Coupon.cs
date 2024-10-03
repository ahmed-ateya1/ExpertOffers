using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Coupon
    {
        public Guid CouponID { get; set; }
        public string CouponCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public string CouponePictureURL { get; set; }
        public string CouponeURL { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public virtual ICollection<SavedItem> SavedItems { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
