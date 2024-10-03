using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.CityDto
{
    public class CityResponse
    {
        public Guid CityID { get; set; }
        public string CityName { get; set; }
        public Guid CountryID { get; set; }
        public string CountryName { get; set; }
    }
}
