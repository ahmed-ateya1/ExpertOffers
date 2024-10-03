using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.DTOS.ClientDto
{
    public class ClientReponse
    {
        public Guid ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
    }
}
