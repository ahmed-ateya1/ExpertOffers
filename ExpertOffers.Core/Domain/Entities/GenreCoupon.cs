using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class GenreCoupon
    {
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }
        public virtual ICollection<Coupon> Coupons { get; set; }
    }
}
