using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Dtos.NotificationDto
{
    public class NotificationResponse
    {
        public Guid NotificationID { get; set; }
        public string Message { get; set; }
        public bool IsRead { get; set; }
        public string NotificationType { get; set; }
        public string? ReferenceURL { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid CompanyID { get; set; }
        public string CompanyName { get; set; }
        public Guid ItemID { get; set; }
        public string ItemImageUrl { get; set; }
        public string ItemTitle { get; set; }
        public double ItemDescount { get; set; }
    }
}
