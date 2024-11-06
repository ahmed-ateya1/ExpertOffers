using System;
using System.Collections.Generic;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Coupon
    {
        public Guid CouponID { get; set; }
        public string CouponTitle { get; set; }
        public string CouponCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string CouponePictureURL { get; set; }
        public string CouponeURL { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public Guid GenreID { get; set; }
        public virtual GenreCoupon GenreCoupon { get; set; }
        public virtual ICollection<SavedItem> SavedItems { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public int GetDaysRemaining()
        {
            TimeSpan timeRemaining = EndDate - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
                return 0;

            return timeRemaining.Days;
        }

        public bool CheckIsActive()
        {
            var currentDate = DateTime.UtcNow;
            return StartDate <= currentDate && EndDate >= currentDate;
        }

    }
}
