using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.SavedItemDto
{
    public class SavedItemResponse
    {
        public Guid SavedItemID { get; set; }
        public Guid ClientID { get; set; }
        public Guid ItemID { get; set; }
        public string ItemType { get; set; }
        public string ItemName { get; set; }
        public string ItemImageURL { get; set; }
        public double? ItemPrice { get; set; }
        public double ItemDiscount { get; set; }
        public int TotalDayRemaining { get; set; }
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyImageURL { get; set; }
        public DateTime SavedAt { get; set; }
    }
}
