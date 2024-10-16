using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.CouponDto
{
    public class CouponAddRequest
    {
        [Required(ErrorMessage = "Coupon Title Can't be Empty")]
        [StringLength(50, ErrorMessage = "Coupon Title Must be Less Than 50 Characters")]
        public string CouponTitle { get; set; }

        [Required(ErrorMessage = "Coupon Code Can't be Empty")]
        [StringLength(50, ErrorMessage = "Coupon Code Must be Less Than 50 Characters")]
        public string CouponCode { get; set; }
        [Required(ErrorMessage = "Discount Percentage Can't be Empty")]
        [Range(1, 100, ErrorMessage = "Discount Percentage Must be Between 1 and 100")]
        public double DiscountPercentage { get; set; }
        [Required(ErrorMessage = "Start Date is Required")]
        public DateTime StartDate { get; set; }
        [Required(ErrorMessage = "End Date is Required")]
        public DateTime EndDate { get; set; }
        [Required(ErrorMessage = "Coupon Picture is Required")]
        public IFormFile CouponePicture { get; set; }

        [Url]
        [Required(ErrorMessage = "Coupon URL is Required")]
        public string CouponeURL { get; set; }
        [Required(ErrorMessage = "Genre ID is Required")]
        public Guid GenreID { get; set; }

        public Guid? CompanyID { get; set; }

    }
}
