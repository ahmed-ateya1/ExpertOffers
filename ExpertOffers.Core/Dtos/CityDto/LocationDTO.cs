using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.CityDto
{
    public class LocationDTO
    {
        [Required(ErrorMessage = "Country is required")]
        public Guid CountryID { get; set; }
        [Required(ErrorMessage = "City is required")]
        public Guid CityID { get; set; }
    }
}
