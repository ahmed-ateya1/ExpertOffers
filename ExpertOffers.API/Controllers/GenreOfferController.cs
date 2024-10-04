using Azure;
using ExpertOffers.Core.Dtos.GenreOffer;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreOfferController : ControllerBase
    {
        private readonly IGenreOfferServices _genreOfferServices;
        private readonly ILogger<GenreOfferController> _logger;

        public GenreOfferController(IGenreOfferServices genreOfferServices, 
            ILogger<GenreOfferController> logger)
        {
            _genreOfferServices = genreOfferServices;
            _logger = logger;
        }
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
