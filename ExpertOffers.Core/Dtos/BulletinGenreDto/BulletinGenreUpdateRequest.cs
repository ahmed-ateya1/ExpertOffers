using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.BulletinGenreDto
{
    public class BulletinGenreUpdateRequest
    {
        [Required(ErrorMessage = "Genre ID Can't be Blank.")]
        public Guid GenreID { get; set; }
        [Required(ErrorMessage = "Genre Name Can't be Blank.")]
        [StringLength(50, ErrorMessage = "Genre Name Can't be More than 50 Character.")]
        public string GenreName { get; set; }
    }
}
