using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Offer
    {
        public Guid OfferID { get; set; }
        public string OfferTitle { get; set; }
        public string OfferDescription { get; set; }
        public double OfferPrice { get; set; }
        public string OfferURL { get; set; }
        public string OfferPictureURL { get; set; }
        public double OfferDiscount { get; set; }
        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public Guid GenreID { get; set; }
        public virtual GenreOffer Genre { get; set; }
        public ICollection<SavedItem> SavedItems { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
    }
}
