using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class GenreOffer
    {
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }
        public string GenreImgURL { get; set; }
        public virtual ICollection<Offer> Offers { get; set; }
    }
}
