using Azure.Core;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.GenreCouponDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing genre coupons.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class GenreCouponController : ControllerBase
    {
        private readonly IGenreCouponServices _genreCouponService;
        private readonly ILogger<GenreCouponController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCouponController"/> class.
        /// </summary>
        /// <param name="genreCouponService">The genre coupon service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public GenreCouponController(
            IGenreCouponServices genreCouponService,
            ILogger<GenreCouponController> logger,
            IUnitOfWork unitOfWork)
        {
            _genreCouponService = genreCouponService;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new genre coupon.
        /// </summary>
        /// <param name="request">The genre coupon add request.</param>
        /// <returns>The created genre coupon.</returns>
        /// <response code="200">Genre coupon added successfully.</response>
        /// <response code="500">An error occurred while creating the genre coupon.</response>
        [HttpPost("createGenreCoupon")]
        public async Task<ActionResult<ApiResponse>> CreateGenreCoupon([FromBody] GenreCouponAddRequest request)
        {
            try
            {
                var genreResponse = await _genreCouponService.CreateAsync(request);
                if (genreResponse == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while creating the genre coupon."
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre coupon added successfully.",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateGenreCoupon method: An error occurred while creating genre coupon.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while creating the genre coupon."
                });
            }
        }

        /// <summary>
        /// Updates an existing genre coupon.
        /// </summary>
        /// <param name="request">The genre coupon update request.</param>
        /// <returns>The updated genre coupon.</returns>
        /// <response code="200">Genre coupon updated successfully.</response>
        /// <response code="404">Genre not found.</response>
        /// <response code="500">An error occurred while updating the genre coupon.</response>
        [HttpPut("updateGenreCoupon")]
        public async Task<ActionResult<ApiResponse>> UpdateGenreCoupon([FromBody] GenreCouponUpdateRequest request)
        {
            try
            {
                var genre = await _unitOfWork.Repository<GenreCoupon>().GetByAsync(x => x.GenreID == request.GenreId);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Genre not found.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var genreResponse = await _genreCouponService.UpdateAsync(request);
                if (genreResponse == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while updating the genre coupon."
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre coupon updated successfully.",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateGenreCoupon method: An error occurred while updating genre coupon.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating the genre coupon."
                });
            }
        }

        /// <summary>
        /// Deletes a genre coupon.
        /// </summary>
        /// <param name="id">The ID of the genre coupon to delete.</param>
        /// <returns>A flag indicating whether the genre coupon was deleted successfully.</returns>
        /// <response code="200">Genre coupon deleted successfully.</response>
        /// <response code="404">Genre not found.</response>
        /// <response code="500">An error occurred while deleting the genre coupon.</response>
        [HttpDelete("deleteGenreCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteGenreCoupon(Guid id)
        {
            try
            {
                var genre = await _unitOfWork.Repository<GenreCoupon>().GetByAsync(x => x.GenreID == id);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Genre not found.",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                var genreResponse = await _genreCouponService.DeleteAsync(id);
                if (!genreResponse)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while deleting the genre coupon."
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre coupon deleted successfully.",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteGenreCoupon method: An error occurred while deleting genre coupon.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting the genre coupon."
                });
            }
        }

        /// <summary>
        /// Gets a genre coupon by ID.
        /// </summary>
        /// <param name="id">The ID of the genre coupon to retrieve.</param>
        /// <returns>The retrieved genre coupon.</returns>
        /// <response code="200">Genre coupon retrieved successfully.</response>
        /// <response code="404">Genre not found.</response>
        /// <response code="500">An error occurred while retrieving the genre coupon.</response>
        [HttpGet("getGenreCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> GetGenreCoupon(Guid id)
        {
            try
            {
                var genre = await _unitOfWork.Repository<GenreCoupon>().GetByAsync(x => x.GenreID == id);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Genre not found.",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var genreResponse = await _genreCouponService.GetByAsync(x => x.GenreID == id);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre coupon retrieved successfully.",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGenreCoupon method: An error occurred while retrieving genre coupon.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving the genre coupon."
                });
            }
        }

        /// <summary>
        /// Gets all genre coupons.
        /// </summary>
        /// <returns>The retrieved genre coupons.</returns>
        /// <response code="200">Genre coupons retrieved successfully.</response>
        /// <response code="500">An error occurred while retrieving the genre coupons.</response>
        [HttpGet("getGenreCoupons")]
        public async Task<ActionResult<ApiResponse>> GetGenreCoupons()
        {
            try
            {
                var genreResponse = await _genreCouponService.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre coupons retrieved successfully.",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGenreCoupons method: An error occurred while retrieving genre coupons.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving the genre coupons."
                });
            }
        }

        /// <summary>
        /// Gets genre coupons by name.
        /// </summary>
        /// <param name="name">The name to search for in genre coupons.</param>
        /// <returns>The retrieved genre coupons.</returns>
        /// <response code="200">Genre coupons retrieved successfully.</response>
        /// <response code="500">An error occurred while retrieving the genre coupons.</response>
        [HttpGet("getGenreCouponsby/{name}")]
        public async Task<ActionResult<ApiResponse>> GetGenreCouponsby(string name)
        {
            try
            {
                var genreResponse = await _genreCouponService.GetAllAsync(x => x.GenreName.ToUpper().Contains(name.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre coupons retrieved successfully.",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGenreCouponsby method: An error occurred while retrieving genre coupons.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving the genre coupons."
                });
            }
        }
    }
}

