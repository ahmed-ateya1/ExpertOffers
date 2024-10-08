using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.FavoriteDto
{
    public class FavoriteAddRequest
    {
        [Required(ErrorMessage = "CompanyID is required")]
        public Guid CompanyID { get; set; }
    }
}
