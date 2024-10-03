using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.CityDto
{
    public class CityUpdateRequest
    {
        [Required(ErrorMessage = "City ID is required")]
        public Guid CityID { get; set; }
        [Required(ErrorMessage = "City Name is required")]
        [StringLength(50, ErrorMessage = "City Name must be less than 50 characters")]
        public string CityName { get; set; }
        [Required(ErrorMessage = "Country ID is required")]
        public Guid CountryID { get; set; }
    }
}
