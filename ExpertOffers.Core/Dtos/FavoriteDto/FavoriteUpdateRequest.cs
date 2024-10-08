using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.FavoriteDto
{
    public class FavoriteUpdateRequest
    {
        public Guid FavoriteID { get; set; }
        public Guid CompanyID { get; set; }
    }
}
