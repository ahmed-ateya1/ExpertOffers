using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.IndustrialDto
{
    public class IndustrialUpdateRequest
    {
        [Required(ErrorMessage = "Industrial ID is required")]
        public Guid IndustrialID { get; set; }
        [Required(ErrorMessage = "Industrial Name is required")]
        [StringLength(50, ErrorMessage = "Industrial Name must be less than 50 characters")]
        public string IndustrialName { get; set; }
    }
}
