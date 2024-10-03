using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Branch
    {
        public Guid BranchID { get; set; }
        public string BranchName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public string BranchLogoURL { get; set; }
        public Guid CompanyID { get; set; }
        public virtual Company Company { get; set; }
    }
}
