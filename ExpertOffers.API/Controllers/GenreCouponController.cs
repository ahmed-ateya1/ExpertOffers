using ExpertOffers.Core.Dtos.GenreCouponDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="GenreCouponController"/> class.
        /// </summary>
        /// <param name="genreCouponService">The genre coupon service.</param>
        /// <param name="logger">The logger.</param>
        public GenreCouponController(
            IGenreCouponServices genreCouponService,
            ILogger<GenreCouponController> logger
            )
        {
            _genreCouponService = genreCouponService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new genre coupon.
        /// </summary>
        /// <param name="request">The genre coupon add request.</param>
        /// <returns>The created genre coupon.</returns>
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
                        Messages = "An error occurred while Create Genre Coupon"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Coupon is added successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateGenreCoupon method: An error occurred while Create genreCoupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Create Genre Coupon"
                });
            }
        }

        /// <summary>
        /// Updates an existing genre coupon.
        /// </summary>
        /// <param name="request">The genre coupon update request.</param>
        /// <returns>The updated genre coupon.</returns>
        [HttpPut("updateGenreCoupon")]
        public async Task<ActionResult<ApiResponse>> UpdateGenreCoupon([FromBody] GenreCouponUpdateRequest request)
        {
            try
            {
                var genreResponse = await _genreCouponService.UpdateAsync(request);
                if (genreResponse == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while Update Genre Coupon"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Coupon is updated successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateGenreCoupon method: An error occurred while Update genreCoupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Update Genre Coupon"
                });
            }
        }

        /// <summary>
        /// Deletes a genre coupon.
        /// </summary>
        /// <param name="id">The ID of the genre coupon to delete.</param>
        /// <returns>A flag indicating whether the genre coupon was deleted successfully.</returns>
        [HttpDelete("deleteGenreCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteGenreCoupon(Guid id)
        {
            try
            {
                var genreResponse = await _genreCouponService.DeleteAsync(id);
                if (!genreResponse)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while Delete Genre Coupon"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Coupon is deleted successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteGenreCoupon method: An error occurred while Delete genreCoupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Delete Genre Coupon"
                });
            }
        }

        /// <summary>
        /// Gets a genre coupon by ID.
        /// </summary>
        /// <param name="id">The ID of the genre coupon to retrieve.</param>
        /// <returns>The retrieved genre coupon.</returns>
        [HttpGet("getGenreCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> GetGenreCoupon(Guid id)
        {
            try
            {
                var genreResponse = await _genreCouponService.GetByAsync(x => x.GenreID == id);
                if (genreResponse == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while Get Genre Coupon"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Coupon is retrieved successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGenreCoupon method: An error occurred while Get genreCoupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get Genre Coupon"
                });
            }
        }

        /// <summary>
        /// Gets all genre coupons.
        /// </summary>
        /// <returns>The retrieved genre coupons.</returns>
        [HttpGet("getGenreCoupons")]
        public async Task<ActionResult<ApiResponse>> GetGenreCoupons()
        {
            try
            {
                var genreResponse = await _genreCouponService.GetAllAsync();
                if (genreResponse == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while Get Genre Coupons"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Coupons are retrieved successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGenreCoupons method: An error occurred while Get genreCoupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get Genre Coupons"
                });
            }
        }

        /// <summary>
        /// Gets genre coupons by name.
        /// </summary>
        /// <param name="name">The name to search for in genre coupons.</param>
        /// <returns>The retrieved genre coupons.</returns>
        [HttpGet("getGenreCouponsby/{name}")]
        public async Task<ActionResult<ApiResponse>> GetGenreCouponsby(string name)
        {
            try
            {
                var genreResponse = await _genreCouponService.GetAllAsync(x => x.GenreName.ToUpper().Contains(name.ToUpper()));
                if (genreResponse == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while Get Genre Coupons"
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Genre Coupons are retrieved successfully",
                    Result = genreResponse,
                    StatusCode = HttpStatusCode.OK
                });

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetGenreCouponsby method: An error occurred while Get genreCoupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Get Genre Coupons"
                });
            }
        }
    }
}
