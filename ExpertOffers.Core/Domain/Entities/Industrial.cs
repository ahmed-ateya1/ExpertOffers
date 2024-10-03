using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Industrial
    {
        public Guid IndustrialID { get; set; }
        public string IndustrialName { get; set; }
        public virtual ICollection<Company> Companies { get; set; }
    }
}
