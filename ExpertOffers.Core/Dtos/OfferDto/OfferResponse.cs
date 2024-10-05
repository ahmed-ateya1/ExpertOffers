using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.OfferDto
{
    public class OfferResponse
    {
        public Guid OfferID { get; set; }
        public string OfferTitle { get; set; }
        public double OfferPrice { get; set; }
        public double OfferPricaBeforeDiscount { get; set; }
        public double OfferDiscount { get; set; }
        public string OfferPictureURL { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long NumOfDaysRemaining { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public bool IsActive { get; set; } 
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoURL { get; set; }
        public Guid genreID { get; set; }
        public string GenreName { get; set; }
    }
}
