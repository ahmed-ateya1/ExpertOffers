using ExpertOffers.Core.Dtos.BulletinGenreDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing bulletin genres.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BulletinGenreController : ControllerBase
    {
        private readonly IBulletinGenreServices _bulletinGenreServices;
        private readonly ILogger<BulletinGenreController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletinGenreController"/> class.
        /// </summary>
        /// <param name="bulletinGenreServices">The bulletin genre services.</param>
        /// <param name="logger">The logger.</param>
        public BulletinGenreController(IBulletinGenreServices bulletinGenreServices,
            ILogger<BulletinGenreController> logger)
        {
            _bulletinGenreServices = bulletinGenreServices;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new bulletin genre.
        /// </summary>
        /// <param name="request">The bulletin genre add request.</param>
        /// <returns>The created bulletin genre.</returns>
        /// <response code="200">Returns the created bulletin genre</response>
        /// <response code="500">Returns an error if creation fails</response>
        [HttpPost("createbulletinGenre")]
        public async Task<ActionResult<ApiResponse>> CreateBulletinGenre([FromBody] BulletinGenreAddRequest request)
        {
            try
            {
                var result = await _bulletinGenreServices.CreateAsync(request);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bulletin Genre created successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateBulletinGenre method: An error occurred while creating bulletin genre.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while creating bulletin genre."
                });
            }
        }

        /// <summary>
        /// Updates an existing bulletin genre.
        /// </summary>
        /// <param name="request">The bulletin genre update request.</param>
        /// <returns>The updated bulletin genre.</returns>
        /// <response code="200">Returns the updated bulletin genre</response>
        /// <response code="500">Returns an error if update fails</response>
        [HttpPut("updateBulletinGenre")]
        public async Task<ActionResult<ApiResponse>> UpdateBulletinGenre([FromBody] BulletinGenreUpdateRequest request)
        {
            try
            {
                var result = await _bulletinGenreServices.UpdateAsync(request);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bulletin Genre updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBulletinGenre method: An error occurred while updating bulletin genre.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating bulletin genre."
                });
            }
        }

        /// <summary>
        /// Deletes a bulletin genre by ID.
        /// </summary>
        /// <param name="genreID">The ID of the bulletin genre to delete.</param>
        /// <returns>The deleted bulletin genre.</returns>
        /// <response code="200">Returns the deleted bulletin genre</response>
        /// <response code="404">If the bulletin genre is not found</response>
        /// <response code="500">Returns an error if deletion fails</response>
        [HttpDelete("deleteBulletinGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> DeleteBulletinGenre(Guid genreID)
        {
            try
            {
                var result = await _bulletinGenreServices.DeleteAsync(genreID);
                if(result == false)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Bulletin Genre not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bulletin Genre deleted successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteBulletinGenre method: An error occurred while deleting bulletin genre.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting bulletin genre."
                });
            }
        }

        /// <summary>
        /// Gets all bulletin genres.
        /// </summary>
        /// <returns>The list of bulletin genres.</returns>
        /// <response code="200">Returns the list of bulletin genres</response>
        /// <response code="500">Returns an error if fetching fails</response>
        [HttpGet("getBulletinGenres")]
        public async Task<ActionResult<ApiResponse>> GetBulletinGenres()
        {
            try
            {
                var result = await _bulletinGenreServices.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bulletin Genres fetched successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBulletinGenres method: An error occurred while fetching bulletin genres.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching bulletin genres."
                });
            }
        }

        /// <summary>
        /// Gets a bulletin genre by ID.
        /// </summary>
        /// <param name="genreID">The ID of the bulletin genre to retrieve.</param>
        /// <returns>The bulletin genre.</returns>
        /// <response code="200">Returns the bulletin genre</response>
        /// <response code="404">If the bulletin genre is not found</response>
        /// <response code="500">Returns an error if fetching fails</response>
        [HttpGet("getBulletinGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinGenre(Guid genreID)
        {
            try
            {
                var result = await _bulletinGenreServices.GetByAsync(x => x.GenreID == genreID);
                if (result == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Bulletin Genre not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bulletin Genre fetched successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBulletinGenre method: An error occurred while fetching bulletin genre.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching bulletin genre."
                });
            }
        }

        /// <summary>
        /// Gets bulletin genres by name.
        /// </summary>
        /// <param name="name">The name to search for.</param>
        /// <returns>The list of bulletin genres matching the name.</returns>
        /// <response code="200">Returns the list of bulletin genres matching the name</response>
        /// <response code="500">Returns an error if fetching fails</response>
        [HttpGet("getBulletinGenresByName/{name}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinGenresByName(string name)
        {
            try
            {
                var result = await _bulletinGenreServices.GetAllAsync(x => x.GenreName.ToUpper().Contains(name.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Bulletin Genres fetched successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBulletinGenresByName method: An error occurred while fetching bulletin genres.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching bulletin genres."
                });
            }
        }
    }
}
