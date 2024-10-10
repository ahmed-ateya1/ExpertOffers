using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.SavedItemDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using ExpertOffers.Infrastructure.UnitOfWorkConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing saved items in the system.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SavedItemController : ControllerBase
    {
        private readonly ISavedItemServices _savedItemServices;
        private readonly ILogger<SavedItemController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="SavedItemController"/> class.
        /// </summary>
        /// <param name="savedItemServices">The service for handling saved items.</param>
        /// <param name="logger">The logger for this controller.</param>
        /// <param name="unitOfWork">The unit of work for database transactions.</param>
        /// <param name="httpContextAccessor">Accessor for HTTP context.</param>
        public SavedItemController(ISavedItemServices savedItemServices,
                                   ILogger<SavedItemController> logger,
                                   IUnitOfWork unitOfWork,
                                   IHttpContextAccessor httpContextAccessor)
        {
            _savedItemServices = savedItemServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Adds a new item to the user's saved items.
        /// </summary>
        /// <param name="savedItem">The saved item request object containing the details of the item to be saved.</param>
        /// <returns>An <see cref="ActionResult"/> containing the result of the operation.</returns>
        /// <response code="200">Indicates the saved item was added successfully.</response>
        /// <response code="500">Indicates an internal server error occurred.</response>
        [Authorize(Roles = "USER")]
        [HttpPost("addToSaved")]
        public async Task<ActionResult<ApiResponse>> AddToSaved(SavedItemAddRequest savedItem)
        {
            try
            {
                var result = await _savedItemServices.CreateAsync(savedItem);
                return Ok(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Saved Item Added Successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "addToSaved method: An error occurred while adding to saved items.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while adding to saved items."
                });
            }
        }

        /// <summary>
        /// Deletes an item from the user's saved items.
        /// </summary>
        /// <param name="savedItemID">The ID of the saved item to delete.</param>
        /// <returns>An <see cref="ActionResult"/> containing the result of the deletion.</returns>
        /// <response code="200">Indicates the saved item was deleted successfully.</response>
        /// <response code="404">Indicates the saved item was not found.</response>
        /// <response code="500">Indicates an internal server error occurred.</response>
        [Authorize(Roles = "USER")]
        [HttpDelete("deleteSavedItem/{savedItemID}")]
        public async Task<ActionResult<ApiResponse>> DeleteSavedItem(Guid savedItemID)
        {
            try
            {
                var saved = await _unitOfWork.Repository<SavedItem>()
                    .GetByAsync(x => x.SavedItemID == savedItemID);
                if (saved == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Saved Item not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var result = await _savedItemServices.DeleteAsync(savedItemID);
                return Ok(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Saved Item Deleted Successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteSavedItem method: An error occurred while deleting the saved item.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting the saved item."
                });
            }
        }

        /// <summary>
        /// Retrieves all saved items for the authenticated user.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> containing the list of saved items.</returns>
        /// <response code="200">Indicates the saved items were retrieved successfully.</response>
        /// <response code="401">Indicates the user is not authenticated.</response>
        /// <response code="404">Indicates the client was not found.</response>
        /// <response code="500">Indicates an internal server error occurred.</response>
        [Authorize(Roles = "USER")]
        [HttpGet("getSavedItems")]
        public async Task<ActionResult<ApiResponse>> GetSavedItems()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext
                    .User.FindFirstValue(ClaimTypes.Email);

                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(u => u.Email == email);

                if (user == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User not authenticated",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }

                var client = await _unitOfWork.Repository<Client>()
                    .GetByAsync(x => x.UserID == user.Id);

                if (client == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Client not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                var result = await _savedItemServices.GetAllAsync(x => x.ClientID == client.ClientID);
                return Ok(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Saved Items Fetched Successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getSavedItems method: An error occurred while retrieving saved items.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving saved items."
                });
            }
        }
    }
}
