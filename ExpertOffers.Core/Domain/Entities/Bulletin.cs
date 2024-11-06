using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Domain.Entities
{
    public class Bulletin
    {
        public Guid BulletinID { get; set; }
        public string BulletinTitle { get; set; }
        public double DiscountPercentage { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string BulletinPictureUrl { get; set; }
        public string BulletinPdfUrl {  get; set; }
        public bool IsActive { get; set; }
        public long TotalViews { get; set; }
        public long TotalSaved { get; set; }
        public Guid CompanyID { get; set; }
        public virtual Company Company { get; set; }
        public Guid GenreID { get; set; }
        public virtual BulletinGenre Genre { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }

        public int GetDaysRemaining()
        {
            TimeSpan timeRemaining = EndDate - DateTime.Now;

            if (timeRemaining.TotalSeconds <= 0)
                return 0;

            return timeRemaining.Days;
        }

        public bool CheckIsActive()
        {
            var currentDate = DateTime.UtcNow;
            return StartDate <= currentDate && EndDate >= currentDate;
        }
    }
}
