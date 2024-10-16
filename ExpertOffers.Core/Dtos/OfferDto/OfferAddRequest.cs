using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.OfferDto
{
    public class OfferAddRequest
    {
        [Required(ErrorMessage = "Offer title is required")]
        [StringLength(100, ErrorMessage = "Offer title must be less than 100 characters")]
        public string OfferTitle { get; set; }

        [Required(ErrorMessage = "genreID is required")]
        public Guid GenreID { get; set; }

        [Required(ErrorMessage = "Offer discount is required")]
        [Range(0, 100, ErrorMessage = "Offer discount must be between 0 and 100")]
        public double OfferDiscount { get; set; }

        [Required(ErrorMessage = "Offer price is required")]
        public double OfferPrice { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Offer Picture is required")]
        public IFormFile OfferPicture { get; set; }

        public Guid? CompanyID { get; set; }
    }
}
