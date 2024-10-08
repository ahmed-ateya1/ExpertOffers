using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class BulletinGenre
    {
        public Guid GenreID { get; set; }
        public string GenreName { get; set; }
        public ICollection<Bulletin> Bulletins { get; set; }
    }
}
