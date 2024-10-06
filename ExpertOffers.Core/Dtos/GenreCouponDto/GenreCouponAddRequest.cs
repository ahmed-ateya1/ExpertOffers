using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.GenreCouponDto
{
    public class GenreCouponAddRequest
    {
        [Required(ErrorMessage = "Genre Name Can't be empty.")]
        public string GenreName { get; set; }

    }
}
