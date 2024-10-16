using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.BulletinDto
{
    public class BulletinAddRquest
    {
        [Required(ErrorMessage = "Bulletin Title is required")]
        [StringLength(100, ErrorMessage = "Bulletin Title cannot be longer than 100 characters")]
        public string BulletinTitle { get; set; }
        [Required(ErrorMessage = "Discount Percentage is required")]
        public double DiscountPercentage { get; set; }
        [Required(ErrorMessage = "Start Date is required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End Date is required")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Bulletin Picture is required")]
        public IFormFile BulletinPicture { get; set; }
        [Required(ErrorMessage = "Bulletin Pdf is required")]
        public IFormFile BulletinPdf { get; set; }
        [Required(ErrorMessage = "Genre ID is required")]
        public Guid GenreID { get; set; }
        public Guid? CompanyID { get; set; }
    }
}
