using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.NotificationDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface INotificationServices
    {
        Task<IEnumerable<NotificationResponse>> GetAllAsync(Expression<Func<Notification, bool>>? expression = null);
        Task<bool> DeleteAsync(Guid notificationID);
    }
}
