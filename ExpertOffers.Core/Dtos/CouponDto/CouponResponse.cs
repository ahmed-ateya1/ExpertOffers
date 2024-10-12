using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.CouponDto
{
    public class CouponResponse
    {
        public Guid CouponID { get; set; }
        public string CouponTitle { get; set; }
        public string CouponCode { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long NumOfDaysRemaining { get; set; }
        public string CouponePictureURL { get; set; }
        public string CouponeURL { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompayLogoURL { get; set; }
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }
        public bool CurrentUserIsSaved { get; set; }


    }
}
