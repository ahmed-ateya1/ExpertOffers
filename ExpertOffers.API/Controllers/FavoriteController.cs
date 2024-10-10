using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.FavoriteDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing favorite operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class FavoriteController : ControllerBase
    {
        private readonly IFavoriteServices _favoriteServices;
        private readonly ILogger<FavoriteController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteController"/> class.
        /// </summary>
        /// <param name="favoriteServices">The favorite services.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public FavoriteController(
            IFavoriteServices favoriteServices,
            ILogger<FavoriteController> logger,
            IUnitOfWork unitOfWork)
        {
            _favoriteServices = favoriteServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a company to favorites.
        /// "USER" role is required to access this endpoint.
        /// </summary>
        /// <param name="favoriteRequest">The favorite request.</param>
        /// <returns>The result of the operation.</returns>
        /// <response code="200">Company added to favorites successfully.</response>
        /// <response code="500">An error occurred while adding the favorite.</response>
        [Authorize(Roles = "USER")]
        [HttpPost("addToFavorite")]
        public async Task<ActionResult<ApiResponse>> AddFavorite([FromBody] FavoriteAddRequest favoriteRequest)
        {
            try
            {
                var result = await _favoriteServices.AddFavorite(favoriteRequest);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Company Add To Favourite successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddFavorite method: An error occurred while adding company to favorite.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while adding the favorite."
                });
            }
        }

        /// <summary>
        /// Removes a company from favorites.
        /// "USER" role is required to access this endpoint.
        /// </summary>
        /// <param name="favoriteID">The ID of the favorite to remove.</param>
        /// <returns>The result of the operation.</returns>
        /// <response code="200">Company removed from favorites successfully.</response>
        /// <response code="404">Favorite not found.</response>
        /// <response code="500">An error occurred while removing the favorite.</response>
        [Authorize(Roles = "USER")]
        [HttpDelete("removeFromFavorite/{favoriteID}")]
        public async Task<ActionResult<ApiResponse>> RemoveFavorite(Guid favoriteID)
        {
            try
            {
                var fav = await _unitOfWork.Repository<Favorite>().GetByAsync(x => x.FavoriteID == favoriteID);
                if (fav == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Favorite not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var result = await _favoriteServices.RemoveFavorite(favoriteID);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Company Remove From Favourite successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "RemoveFavorite method: An error occurred while removing company from favorite.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while removing the favorite."
                });
            }
        }

        /// <summary>
        /// Gets all favorites for a client.
        /// </summary>
        /// <param name="clientID">The ID of the client.</param>
        /// <returns>The result of the operation.</returns>
        /// <response code="200">Favorites retrieved successfully.</response>
        /// <response code="500">An error occurred while retrieving favorites.</response>
        [HttpGet("getAllFavorites/{clientID}")]
        public async Task<ActionResult<ApiResponse>> GetAllFavorites(Guid clientID)
        {
            try
            {
                var result = await _favoriteServices.GetAllAsync(x => x.ClientID == clientID);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Get All Favorites successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAllFavorites method: An error occurred while getting all favorites.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting all favorites."
                });
            }
        }
    }
}

