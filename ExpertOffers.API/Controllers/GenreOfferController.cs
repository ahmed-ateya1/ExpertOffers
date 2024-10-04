using Azure;
using ExpertOffers.Core.Dtos.GenreOffer;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// API Controller for managing Genre Offers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GenreOfferController : ControllerBase
    {
        private readonly IGenreOfferServices _genreOfferServices;
        private readonly ILogger<GenreOfferController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreOfferController"/> class.
        /// </summary>
        /// <param name="genreOfferServices">The genre offer services.</param>
        /// <param name="logger">The logger.</param>
        public GenreOfferController(IGenreOfferServices genreOfferServices, ILogger<GenreOfferController> logger)
        {
            _genreOfferServices = genreOfferServices;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new genre offer.
        /// </summary>
        /// <param name="genreAdd">The genre addition request.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        [HttpPost("createGenre")]
        public async Task<ActionResult<ApiResponse>> CreateGenre([FromForm] GenreAddRequest genreAdd)
        {
            try
            {
                var genreResponse = await _genreOfferServices.CreateAsync(genreAdd);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Offer is added successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "createGenre method: An error occurred while Create Genre");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Create Genre"
                });
            }
        }

        /// <summary>
        /// Updates an existing genre offer.
        /// </summary>
        /// <param name="genreUpdate">The genre update request.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        [HttpPut("updateGenre")]
        public async Task<ActionResult<ApiResponse>> UpdateGenre([FromForm] GenreUpdateRequest genreUpdate)
        {
            try
            {
                var genreResponse = await _genreOfferServices.UpdateAsync(genreUpdate);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Offer is updated successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateGenre method: An error occurred while Update Genre");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Update Genre"
                });
            }
        }

        /// <summary>
        /// Deletes a genre offer by ID.
        /// </summary>
        /// <param name="id">The genre ID.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        [HttpDelete("deleteGenre/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteGenre(Guid id)
        {
            try
            {
                var isDeleted = await _genreOfferServices.DeleteAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Genre Offer is not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Offer is deleted successfully",
                    Result = isDeleted,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteGenre method: An error occurred while Delete Genre");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Delete Genre"
                });
            }
        }

        /// <summary>
        /// Gets all genre offers.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        [HttpGet("getGenres")]
        public async Task<ActionResult<ApiResponse>> GetGenres()
        {
            try
            {
                var genres = await _genreOfferServices.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Offers are fetched successfully",
                    Result = genres,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getGenres method: An error occurred while Get Genres");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get Genres"
                });
            }
        }

        /// <summary>
        /// Gets a genre offer by ID.
        /// </summary>
        /// <param name="id">The genre ID.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        [HttpGet("getGenre/{id}")]
        public async Task<ActionResult<ApiResponse>> GetGenre(Guid id)
        {
            try
            {
                var genre = await _genreOfferServices.GetByAsync(x => x.GenreID == id);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Genre Offer is not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Offer is fetched successfully",
                    Result = genre,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getGenre method: An error occurred while Get Genre");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get Genre"
                });
            }
        }

        /// <summary>
        /// Gets genre offers by name.
        /// </summary>
        /// <param name="name">The genre name.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        [HttpGet("getGenresBy/{name}")]
        public async Task<ActionResult<ApiResponse>> GetGenresBy(string name)
        {
            try
            {
                var genres = await _genreOfferServices.GetAllAsync(x => x.GenreName.ToUpper().Contains(name.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Offers are fetched successfully",
                    Result = genres,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getGenresBy method: An error occurred while Get Genres By");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get Genres By"
                });
            }
        }
    }
}
