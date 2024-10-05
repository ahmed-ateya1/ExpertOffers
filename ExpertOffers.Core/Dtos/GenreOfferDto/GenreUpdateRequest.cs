using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.GenreOffer
{
    public class GenreUpdateRequest
    {
        [Required(ErrorMessage = "genreID ID is required")]
        public Guid GenreID { get; set; }

        [Required(ErrorMessage = "genreID Name is required")]
        [StringLength(50, ErrorMessage = "genreID Name must be less than 50 characters")]
        public string GenreName { get; set; }

        public IFormFile? GenreImg { get; set; }
    }
}
