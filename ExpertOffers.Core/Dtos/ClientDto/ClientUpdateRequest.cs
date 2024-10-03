using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.ClientDto
{
    public class ClientUpdateRequest
    {
        [Required(ErrorMessage = "First Name is required.")]
        [StringLength(50, ErrorMessage = "First Name can't be longer than 50 characters.")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required.")]
        [StringLength(50, ErrorMessage = "Last Name can't be longer than 50 characters.")]
        public string LastName { get; set; }
        [Required(ErrorMessage = "Phone Number is required.")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone Number is not valid. It should be in international format.")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Country is required.")]
        public Guid CountryID { get; set; }
        [Required(ErrorMessage = "City is required.")]
        public Guid CityID { get; set; }

    }
}
