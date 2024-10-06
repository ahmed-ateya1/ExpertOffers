using ExpertOffers.Core.Dtos.CouponDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{

    /// <summary>
    /// Represents the controller for managing coupons.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponServices _couponServices;
        private readonly ILogger<CouponController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponController"/> class.
        /// </summary>
        /// <param name="couponServices">The coupon services.</param>
        /// <param name="logger">The logger.</param>
        public CouponController(
            ICouponServices couponServices,
            ILogger<CouponController> logger)
        {
            _couponServices = couponServices;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new coupon.
        /// </summary>
        /// <param name="couponAddRequest">The coupon add request.</param>
        /// <returns>The created coupon.</returns>
        [Authorize(Roles = "COMPANY")]
        [HttpPost("createCoupon")]
        public async Task<ActionResult<ApiResponse>> CreateCoupon([FromForm] CouponAddRequest couponAddRequest)
        {
            try
            {
                var response = await _couponServices.CreateAsync(couponAddRequest);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupon created successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "createCoupon method: An error occurred while Create Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while create Coupon"
                });
            }
        }

        /// <summary>
        /// Updates an existing coupon.
        /// </summary>
        /// <param name="couponUpdateRequest">The coupon update request.</param>
        /// <returns>The updated coupon.</returns>
        [Authorize(Roles = "COMPANY")]
        [HttpPut("updateCoupon")]
        public async Task<ActionResult<ApiResponse>> UpdateCoupon([FromForm] CouponUpdateRequest couponUpdateRequest)
        {
            try
            {
                var response = await _couponServices.UpdateAsync(couponUpdateRequest);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupon updated successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateCoupon method: An error occurred while Update Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while update Coupon"
                });
            }
        }

        /// <summary>
        /// Deletes a coupon by its ID.
        /// </summary>
        /// <param name="id">The ID of the coupon to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        [Authorize(Roles = "COMPANY")]
        [HttpDelete("deleteCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCoupon(Guid id)
        {
            try
            {
                var response = await _couponServices.DeleteAsync(id);
                if (!response)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Coupon not found"
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupon deleted successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteCoupon method: An error occurred while Delete Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while delete Coupon"
                });
            }
        }

        /// <summary>
        /// Gets a coupon by its ID.
        /// </summary>
        /// <param name="id">The ID of the coupon to get.</param>
        /// <returns>The found coupon.</returns>
        [HttpGet("getCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> GetCoupon(Guid id)
        {
            try
            {
                var result = await _couponServices.GetByAsync(c => c.CouponID == id);
                if (result == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Coupon not found"
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupon found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getCoupon method: An error occurred while Get Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupon"
                });
            }
        }
        /// <summary>
        /// Gets coupons by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The coupons found.</returns>
        [HttpGet("getCouponsByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetCouponByCompany(Guid companyID)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.CouponID == companyID);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCouponByCompany method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }
        /// <summary>
        /// Gets coupons by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The coupons found.</returns>
        [HttpGet("getCouponsByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetCouponByGenre(Guid genreID)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.GenreID == genreID);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCouponByGenre method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }
        /// <summary>
        /// Gets all coupons.
        /// </summary>
        /// <returns>All the coupons found.</returns>
        [HttpGet("getAllCoupons")]
        public async Task<ActionResult<ApiResponse>> GetAllCoupons()
        {
            try
            {
                var result = await _couponServices.GetAllAsync();
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getAllCoupons method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets active coupons.
        /// </summary>
        /// <returns>The active coupons found.</returns>
        [HttpGet("getActiveCoupons")]
        public async Task<ActionResult<ApiResponse>> GetActiveCoupons()
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.IsActive == true);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getActiveCoupons method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets inactive coupons.
        /// </summary>
        /// <returns>The inactive coupons found.</returns>
        [HttpGet("getInactiveCoupons")]
        public async Task<ActionResult<ApiResponse>> GetInactiveCoupons()
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.IsActive == false);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getInactiveCoupons method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets active coupons by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The active coupons found.</returns>
        [HttpGet("getActiveCouponsByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetActiveCouponsByCompany(Guid companyID)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.CompanyID == companyID && x.IsActive == true);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getActiveCouponsByCompany method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets inactive coupons by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The inactive coupons found.</returns>
        [HttpGet("getInactiveCouponsByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetInActiveCouponsByCompany(Guid companyID)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.CompanyID == companyID && x.IsActive == false);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getActiveCouponsByCompany method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets active coupons by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The active coupons found.</returns>
        [HttpGet("getActiveCouponsByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetActiveCouponsByGenre(Guid genreID)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.GenreID == genreID && x.IsActive == true);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getActiveCouponsByCompany method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets inactive coupons by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The inactive coupons found.</returns>
        [HttpGet("getInActiveCouponsByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetInActiveCouponsByGenre(Guid genreID)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.GenreID == genreID && x.IsActive == false);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getActiveCouponsByCompany method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }

        /// <summary>
        /// Gets coupons by title.
        /// </summary>
        /// <param name="title">The title of the coupons.</param>
        /// <returns>The coupons found.</returns>
        [HttpGet("getCouponsByTitle/{title}")]
        public async Task<ActionResult<ApiResponse>> GetCouponsByTitle(string title)
        {
            try
            {
                var result = await _couponServices.GetAllAsync(x => x.CouponTitle.ToUpper().Contains(title.ToUpper()));
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Coupons found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getActiveCouponsByCompany method: An error occurred while Get Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Coupons"
                });
            }
        }
    }
}