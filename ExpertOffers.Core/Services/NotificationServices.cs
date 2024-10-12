using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.NotificationDto;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using System.Linq.Expressions;

public class NotificationServices : INotificationServices
{
    private readonly IUnitOfWork _unitOfWork;

    public NotificationServices(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    private NotificationResponse HandleCommonNotification(Notification notification, string itemTitle, string itemImageUrl, double? itemDiscount, Guid? itemId, Company company)
    {
        return new NotificationResponse
        {
            NotificationID = notification.NotificationID,
            Message = notification.Message,
            NotificationType = notification.NotificationType,
            ReferenceURL = notification.ReferenceURL,
            CreatedDate = notification.CreatedDate,
            ItemTitle = itemTitle,
            ItemImageUrl = itemImageUrl,
            ItemDescount = itemDiscount ?? 0,
            ItemID = itemId ?? Guid.Empty,
            IsRead = notification.IsRead,
            CompanyID = company.CompanyID,
            CompanyName = company.CompanyName
        };
    }

    private NotificationResponse HandleNotificationByType(Notification notification)
    {
        notification.IsRead = true;
        if (notification.Offer != null)
        {
            return HandleCommonNotification(notification, notification.Offer.OfferTitle, notification.Offer.OfferPictureURL, notification.Offer.OfferDiscount, notification.Offer.OfferID, notification.Offer.Company);
        }
        else if (notification.Coupon != null)
        {
            return HandleCommonNotification(notification, notification.Coupon.CouponTitle, notification.Coupon.CouponePictureURL, notification.Coupon.DiscountPercentage, notification.Coupon.CouponID, notification.Coupon.Company);
        }
        else if (notification.Bulletin != null)
        {
            return HandleCommonNotification(notification, notification.Bulletin.BulletinTitle, notification.Bulletin.BulletinPictureUrl, notification.Bulletin.DiscountPercentage, notification.Bulletin.BulletinID, notification.Bulletin.Company);
        }

        throw new InvalidOperationException("Unknown notification type.");
    }

    private async Task<List<NotificationResponse>> SetNotificationsAsync(IEnumerable<Notification> notifications)
    {
        var notificationResponse = new List<NotificationResponse>();

        foreach (var notification in notifications)
        {
            notificationResponse.Add(HandleNotificationByType(notification));
        }
        await _unitOfWork.CompleteAsync();
        return notificationResponse;
    }

    public async Task<IEnumerable<NotificationResponse>> GetAllAsync(Expression<Func<Notification, bool>>? expression = null)
    {
        var notifications = await _unitOfWork.Repository<Notification>()
            .GetAllAsync(expression, includeProperties: "Client,Offer.Company,Coupon.Company,Bulletin.Company");

        return await SetNotificationsAsync(notifications);
    }
}
