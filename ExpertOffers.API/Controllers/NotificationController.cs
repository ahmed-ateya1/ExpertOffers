using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.NotificationDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ExpertOffers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationServices _notificationServices;
        private readonly ILogger<NotificationController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public NotificationController(INotificationServices notificationServices
            , ILogger<NotificationController> logger
            , IUnitOfWork unitOfWork
            ,IHttpContextAccessor httpContextAccessor)
        {
            _notificationServices = notificationServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Deletes a notification by its ID.
        /// </summary>
        /// <remarks>
        /// This endpoint deletes a specific notification from the system using its unique identifier (NotificationID). 
        /// It requires the user to be authorized. If the notification is not found, a 404 status code is returned.
        /// </remarks>
        /// <param name="notificationID">The unique identifier of the notification to be deleted.</param>
        /// <returns>
        /// An ActionResult containing the result of the operation:
        /// - 200 OK: If the notification was successfully deleted.
        /// - 404 Not Found: If the notification with the given ID does not exist.
        /// - 500 Internal Server Error: If an error occurred on the server while processing the request.
        /// </returns>
        /// <response code="200">Notification deleted successfully.</response>
        /// <response code="404">Notification not found.</response>
        /// <response code="500">An error occurred while processing your request.</response>
        [HttpDelete("deleteNotifications/{notificationID}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> DeleteNotification(Guid notificationID)
        {
            try
            {
                var result = await _notificationServices.DeleteAsync(notificationID);
                if (!result)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Notification not found",
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Notification deleted successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while processing your request"
                });
            }
        }
        /// <summary>
        /// Fetches all notifications for the authenticated user.
        /// </summary>
        /// <remarks>
        /// Note this endpoint not work in real time if you want to work in real time used notificationhub
        /// This endpoint retrieves all notifications for the currently authenticated user. 
        /// The user can either be a client or a company representative, and the relevant notifications 
        /// are fetched based on their ID. The user must be authenticated to use this endpoint.
        /// </remarks>
        /// <returns>
        /// An ActionResult containing the user's notifications:
        /// - 200 OK: If the notifications are successfully retrieved.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// - 404 Not Found: If the user is not found.
        /// - 500 Internal Server Error: If an error occurred on the server while processing the request.
        /// </returns>
        /// <response code="200">Notifications fetched successfully.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="404">User not found.</response>
        /// <response code="500">An error occurred while processing your request.</response>
        [HttpGet("getNotifications")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> GetNotifications()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User not authenticated"
                    });
                }

                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(x => x.Email == email,includeProperties:"Client,Company");
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User not found"
                    });
                }

                IEnumerable<NotificationResponse> notifications;
                if (user.ClientID != null)
                {
                    notifications = await _notificationServices.GetAllAsync(x => x.ClientID == user.ClientID);
                }
                else if (user.ComapnyID != null)
                {
                    notifications = await _notificationServices.GetAllAsync(x => x.CompanyID == user.ComapnyID);
                }
                else
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User does not belong to a client or company"
                    });
                }

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Notifications fetched successfully",
                    Result = notifications
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError, new ApiResponse
                {
                    IsSuccess = false,
                    Messages = "An error occurred while processing your request"
                });
            }
        }


    }

}
