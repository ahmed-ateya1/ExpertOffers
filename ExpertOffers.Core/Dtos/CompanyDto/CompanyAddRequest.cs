using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.CompanyDto
{
    public class CompanyAddRequest
    {
        [Required(ErrorMessage = "Company Name is required")]
        [StringLength(50, ErrorMessage = "Company Name must be less than 50 characters")]
        public string CompanyName { get; set; }
        [Required(ErrorMessage = "Company Logo URL is required")]
        public IFormFile CompanyLogo { get; set; }
        [Required(ErrorMessage = "Industrial ID is required")]
        public Guid IndustrialID { get; set; }
    }
}
