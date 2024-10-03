using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.BranchDto
{
    public class BranchUpdateRequest
    {
        [Required(ErrorMessage = "Branch ID is required")]
        public Guid BranchID { get; set; }
        [Required(ErrorMessage = "Company ID is required")]
        public Guid CompanyID { get; set; }
        [Required(ErrorMessage = "Branch Name is required")]
        [StringLength(50, ErrorMessage = "Branch Name must be less than 50 characters")]
        public string BranchName { get; set; }
        [Required(ErrorMessage = "Phone Number is required")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Phone Number is not valid. It should be in international format.")]
        public string PhoneNumber { get; set; }
        [Required(ErrorMessage = "Location is required")]
        public string Location { get; set; }

        public IFormFile? BranchLogo { get; set; }
    }
}
