using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.NotificationDto;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace ExpertOffers.Core.Hubs
{
    /// <summary>
    /// Represents a SignalR hub for handling notifications.
    /// </summary>
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly INotificationServices _notificationServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;

        private static Dictionary<string, string> _connections = new Dictionary<string, string>();

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationHub"/> class.
        /// </summary>
        public NotificationHub(
            INotificationServices notificationServices,
            UserManager<ApplicationUser> userManager,
            IHttpContextAccessor httpContextAccessor,
            IUnitOfWork unitOfWork)
        {
            _notificationServices = notificationServices;
            _userManager = userManager;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Called when a client is connected to the hub.
        /// </summary>
        public override async Task OnConnectedAsync()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
            {
                _connections[Context.ConnectionId] = email;

                //var notifications = await FetchNotifications(email);

                //await Clients.Client(Context.ConnectionId).SendAsync("ReceiveNotifications", notifications);

                int unreadCount = await GetUnreadNotificationCount();
                await Clients.Client(Context.ConnectionId).SendAsync("ReceiveUnreadNotificationCount", unreadCount);
            }
            await base.OnConnectedAsync();
        }

        /// <summary>
        /// Called when a client is disconnected from the hub.
        /// </summary>
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            _connections.Remove(Context.ConnectionId);
            await base.OnDisconnectedAsync(exception);
        }

        /// <summary>
        /// Gets the number of unread notifications for the connected user.
        /// </summary>
        public async Task<int> GetUnreadNotificationCount()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (!string.IsNullOrEmpty(email))
            {
                var client = await _unitOfWork.Repository<Client>().GetByAsync(x => x.User.Email == email, includeProperties: "User");

                if (client != null)
                {
                    return await _unitOfWork.Repository<Notification>().CountAsync(x => x.ClientID == client.ClientID && !x.IsRead);
                }
                else
                {
                    var company = await _unitOfWork.Repository<Company>().GetByAsync(x => x.User.Email == email, includeProperties: "User");

                    if (company != null)
                    {
                        return await _unitOfWork.Repository<Notification>().CountAsync(x => x.CompanyID == company.CompanyID && !x.IsRead);
                    }
                }
            }
            return 0;
        }

        /// <summary>
        /// Sends a notification to a specific user.
        /// </summary>
        public async Task SendNotificationToUser(string connectionId, Notification notification)
        {
            if (_connections.ContainsKey(connectionId))
            {
                await Clients.Client(connectionId).SendAsync("ReceiveNotification", notification);

                // Update the unread notification count if necessary
                int unreadCount = await GetUnreadNotificationCount();
                await Clients.Client(connectionId).SendAsync("ReceiveUnreadNotificationCount", unreadCount);
            }
        }

        /// <summary>
        /// Fetches notifications for a specific email.
        /// </summary>
        private async Task<List<NotificationResponse>> FetchNotifications(string email)
        {
            var client = await _unitOfWork.Repository<Client>().GetByAsync(x => x.User.Email == email, includeProperties: "User");
            if (client != null)
            {
                return (List<NotificationResponse>)await _notificationServices.GetAllAsync(x => x.ClientID == client.ClientID);
            }

            var company = await _unitOfWork.Repository<Company>().GetByAsync(x => x.User.Email == email, includeProperties: "User");
            if (company != null)
            {
                return (List<NotificationResponse>)await _notificationServices.GetAllAsync(x => x.CompanyID == company.CompanyID);
            }

            return new List<NotificationResponse>();
        }
    }
}
