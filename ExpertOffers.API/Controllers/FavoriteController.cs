using ExpertOffers.Core.Dtos.FavoriteDto;
using ExpertOffers.Core.DTOS;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="FavoriteController"/> class.
        /// </summary>
        /// <param name="favoriteServices">The favorite services.</param>
        /// <param name="logger">The logger.</param>
        public FavoriteController(
            IFavoriteServices favoriteServices,
            ILogger<FavoriteController> logger
            )
        {
            _favoriteServices = favoriteServices;
            _logger = logger;
        }

        /// <summary>
        /// Adds a company to favorites.
        /// </summary>
        /// <param name="favoriteRequest">The favorite request.</param>
        /// <returns>The result of the operation.</returns>
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
                _logger.LogError(ex, "AddFavorite method: An error occurred while Add company to favorite.");
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
        /// </summary>
        /// <param name="favoriteID">The ID of the favorite to remove.</param>
        /// <returns>The result of the operation.</returns>
        [Authorize(Roles = "USER")]
        [HttpDelete("removeFromFavorite/{favoriteID}")]
        public async Task<ActionResult<ApiResponse>> RemoveFavorite(Guid favoriteID)
        {
            try
            {
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
