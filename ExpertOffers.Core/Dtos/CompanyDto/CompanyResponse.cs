using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.CompanyDto
{
    public class CompanyResponse
    {
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string CompanyLogoURL { get; set; }
        public string CountryName { get; set; }
        public string CityName { get; set; }
        public string IndustrialName { get; set; }

        public long NumberOfBulletins { get; set; }
        public bool IsFavoriteToCurrentUser { get; set; }
    }
}
