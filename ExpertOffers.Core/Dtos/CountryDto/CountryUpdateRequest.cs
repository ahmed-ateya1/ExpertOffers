using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.CountryDto
{
    public class CountryUpdateRequest
    {
        [Required(ErrorMessage = "Country ID is required")]
        public Guid CountryID { get; set; }
        [Required(ErrorMessage = "Country Name is required")]
        [StringLength(50, ErrorMessage = "Country Name must be less than 50 characters")]
        public string CountryName { get; set; }
    }
}
