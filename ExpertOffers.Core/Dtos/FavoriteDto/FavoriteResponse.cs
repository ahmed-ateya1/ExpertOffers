using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.FavoriteDto
{
    public class FavoriteResponse
    {
        public Guid FavoriteID { get; set; }
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogo { get; set; }
        public Guid ClientID { get; set; }
        public long TotalBulletinForCompany { get; set; }
        public DateTime DateAdded { get; set; }
    }
}
