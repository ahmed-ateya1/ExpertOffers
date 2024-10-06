using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.BranchDto
{
    public class BranchResponse
    {
        public Guid BranchID { get; set; }
        public string BranchName { get; set; }
        public string PhoneNumber { get; set; }
        public string Location { get; set; }
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyLogoURL { get; set; }
    }
}
