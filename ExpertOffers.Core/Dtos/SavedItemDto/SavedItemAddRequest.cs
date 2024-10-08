using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.SavedItemDto
{
    public class SavedItemAddRequest
    {
        [Required(ErrorMessage ="Item ID is required.")]
        public Guid ItemID { get; set; }
    }
}
