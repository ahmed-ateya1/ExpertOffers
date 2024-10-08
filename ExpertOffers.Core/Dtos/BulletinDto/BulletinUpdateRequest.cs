using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.BulletinDto
{
    public class BulletinUpdateRequest
    {
        [Required(ErrorMessage = "Bulletin ID is required")]
        public Guid BulletinID { get; set; }
        [Required(ErrorMessage = "Bulletin Title is required")]
        [StringLength(100, ErrorMessage = "Bulletin Title cannot be longer than 100 characters")]
        public string BulletinTitle { get; set; }
        [Required(ErrorMessage = "Discount Percentage is required")]
        public double DiscountPercentage { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }
        public IFormFile? BulletinPicture { get; set; }
        public IFormFile? BulletinPdf { get; set; }
        [Required(ErrorMessage = "Genre ID is required")]
        public Guid GenreID { get; set; }
    }
}
