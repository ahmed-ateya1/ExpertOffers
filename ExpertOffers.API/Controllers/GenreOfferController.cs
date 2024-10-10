using Azure;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreOffer;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
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
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreOfferController"/> class.
        /// </summary>
        /// <param name="genreOfferServices">The genre offer services.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="unitOfWork"></param>
        public GenreOfferController(IGenreOfferServices genreOfferServices, ILogger<GenreOfferController> logger, IUnitOfWork unitOfWork)
        {
            _genreOfferServices = genreOfferServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new genre offer.
        /// </summary>
        /// <param name="genreAdd">The genre addition request.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        /// <response code="200">Returns the created genre offer.</response>
        /// <response code="500">Returns an error message if an unexpected error occurs.</response>
        [HttpPost("createGenre")]
        public async Task<ActionResult<ApiResponse>> CreateGenre([FromForm] GenreAddRequest genreAdd)
        {
            try
            {
                var genreResponse = await _genreOfferServices.CreateAsync(genreAdd);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "genreID Offer is added successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "createGenre method: An error occurred while Create genreID");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Create genreID"
                });
            }
        }

        /// <summary>
        /// Updates an existing genre offer.
        /// </summary>
        /// <param name="genreUpdate">The genre update request.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        /// <response code="200">Returns the updated genre offer.</response>
        /// <response code="404">Returns an error message if the genre is not found.</response>
        /// <response code="500">Returns an error message if an unexpected error occurs.</response>
        [HttpPut("updateGenre")]
        public async Task<ActionResult<ApiResponse>> UpdateGenre([FromForm] GenreUpdateRequest genreUpdate)
        {
            try
            {
                var genre = await _unitOfWork.Repository<GenreOffer>()
                    .GetByAsync(x => x.GenreID == genreUpdate.GenreID);

                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Genre Not Found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                var genreResponse = await _genreOfferServices.UpdateAsync(genreUpdate);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "genreID Offer is updated successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateGenre method: An error occurred while Update genreID");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Update genreID"
                });
            }
        }

        /// <summary>
        /// Deletes a genre offer by ID.
        /// </summary>
        /// <param name="id">The genre ID.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        /// <response code="200">Returns a success message if the genre offer is deleted.</response>
        /// <response code="404">Returns an error message if the genre offer is not found.</response>
        /// <response code="500">Returns an error message if an unexpected error occurs.</response>
        [HttpDelete("deleteGenre/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteGenre(Guid id)
        {
            try
            {
                var genre = await _unitOfWork.Repository<GenreOffer>()
                    .GetByAsync(x => x.GenreID == id);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "genreID Offer is not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var isDeleted = await _genreOfferServices.DeleteAsync(id);
                if (!isDeleted)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "genreID Offer is not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "genreID Offer is deleted successfully",
                    Result = isDeleted,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteGenre method: An error occurred while Delete genreID");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Delete genreID"
                });
            }
        }

        /// <summary>
        /// Gets all genre offers.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        /// <response code="200">Returns the list of genre offers.</response>
        /// <response code="500">Returns an error message if an unexpected error occurs.</response>
        [HttpGet("getGenres")]
        public async Task<ActionResult<ApiResponse>> GetGenres()
        {
            try
            {
                var genres = await _genreOfferServices.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "genreID Offers are fetched successfully",
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
        /// <response code="200">Returns the genre offer.</response>
        /// <response code="404">Returns an error message if the genre offer is not found.</response>
        /// <response code="500">Returns an error message if an unexpected error occurs.</response>
        [HttpGet("getGenre/{id}")]
        public async Task<ActionResult<ApiResponse>> GetGenre(Guid id)
        {
            try
            {
                var genreF = await _unitOfWork.Repository<GenreOffer>()
                    .GetByAsync(x => x.GenreID == id);
                if (genreF == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "genreID Offer is not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var genre = await _genreOfferServices.GetByAsync(x => x.GenreID == id);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "genreID Offer is not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "genreID Offer is fetched successfully",
                    Result = genre,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getGenre method: An error occurred while Get genreID");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get genreID"
                });
            }
        }

        /// <summary>
        /// Gets genre offers by name.
        /// </summary>
        /// <param name="name">The genre name.</param>
        /// <returns>An <see cref="ActionResult"/> containing the response.</returns>
        /// <response code="200">Returns the list of genre offers matching the name.</response>
        /// <response code="500">Returns an error message if an unexpected error occurs.</response>
        [HttpGet("getGenresBy/{name}")]
        public async Task<ActionResult<ApiResponse>> GetGenresBy(string name)
        {
            try
            {
                var genres = await _genreOfferServices.GetAllAsync(x => x.GenreName.ToUpper().Contains(name.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "genreID Offers are fetched successfully",
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
