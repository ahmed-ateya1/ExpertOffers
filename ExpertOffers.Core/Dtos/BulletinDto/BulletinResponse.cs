using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.BulletinDto
{
    public class BulletinResponse
    {
        public Guid BulletinID { get; set; }
        public string BulletinTitle { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public long NumOfDaysRemaining { get; set; }
        public double DiscountPercentage { get; set; }
        public string BulletinPictureUrl { get; set; }
        public string BulletinPdfUrl { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public bool IsActive { get; set; }
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoURL { get; set; }
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }

    }
}
