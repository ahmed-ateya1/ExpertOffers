using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.CouponDto;
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
    /// Represents the controller for managing coupons.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CouponController : ControllerBase
    {
        private readonly ICouponServices _couponServices;
        private readonly ILogger<CouponController> _logger;
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Initializes a new instance of the <see cref="CouponController"/> class.
        /// </summary>
        /// <param name="couponServices">The coupon services.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        public CouponController(
            ICouponServices couponServices,
            ILogger<CouponController> logger,
            IUnitOfWork unitOfWork)
        {
            _couponServices = couponServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Creates a new coupon.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="couponAddRequest">The coupon add request.</param>
        /// <returns>The created coupon.</returns>
        /// <response code="200">Coupon created successfully.</response>
        /// <response code="500">An error occurred while creating the coupon.</response>
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
                _logger.LogError(ex, "createCoupon method: An error occurred while creating Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while creating Coupon"
                });
            }
        }

        /// <summary>
        /// Updates an existing coupon.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="couponUpdateRequest">The coupon update request.</param>
        /// <returns>The updated coupon.</returns>
        /// <response code="200">Coupon updated successfully.</response>
        /// <response code="404">Coupon not found.</response>
        /// <response code="500">An error occurred while updating the coupon.</response>
        [Authorize(Roles = "COMPANY")]
        [HttpPut("updateCoupon")]
        public async Task<ActionResult<ApiResponse>> UpdateCoupon([FromForm] CouponUpdateRequest couponUpdateRequest)
        {
            try
            {
                var coupon = await _unitOfWork.Repository<Coupon>()
                    .GetByAsync(x => x.CouponID == couponUpdateRequest.CouponID);
                if (coupon == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Coupon not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
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
                _logger.LogError(ex, "updateCoupon method: An error occurred while updating Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Coupon"
                });
            }
        }

        /// <summary>
        /// Deletes a coupon by its ID.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="id">The ID of the coupon to delete.</param>
        /// <returns>The result of the delete operation.</returns>
        /// <response code="200">Coupon deleted successfully.</response>
        /// <response code="404">Coupon not found.</response>
        /// <response code="500">An error occurred while deleting the coupon.</response>
        [Authorize(Roles = "COMPANY")]
        [HttpDelete("deleteCoupon/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteCoupon(Guid id)
        {
            try
            {
                var coupon = await _unitOfWork.Repository<Coupon>()
                    .GetByAsync(x => x.CouponID == id);
                if (coupon == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Coupon not found"
                    });
                }
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
                _logger.LogError(ex, "deleteCoupon method: An error occurred while deleting Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting Coupon"
                });
            }
        }

        /// <summary>
        /// Gets a coupon by its ID.
        /// </summary>
        /// <param name="id">The ID of the coupon to get.</param>
        /// <returns>The found coupon.</returns>
        /// <response code="200">Coupon found successfully.</response>
        /// <response code="404">Coupon not found.</response>
        /// <response code="500">An error occurred while retrieving the coupon.</response>
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
                _logger.LogError(ex, "getCoupon method: An error occurred while retrieving Coupon");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Coupon"
                });
            }
        }

        /// <summary>
        /// Gets coupons by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The coupons found.</returns>
        /// <response code="200">Coupons found successfully.</response>
        /// <response code="500">An error occurred while retrieving the coupons.</response>
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
                _logger.LogError(ex, "GetCouponByCompany method: An error occurred while retrieving Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Coupons"
                });
            }
        }

        /// <summary>
        /// Gets coupons by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The coupons found.</returns>
        /// <response code="200">Coupons found successfully.</response>
        /// <response code="500">An error occurred while retrieving the coupons.</response>
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
                _logger.LogError(ex, "GetCouponByGenre method: An error occurred while retrieving Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Coupons"
                });
            }
        }

        /// <summary>
        /// Gets all coupons.
        /// </summary>
        /// <returns>All the coupons found.</returns>
        /// <response code="200">Coupons found successfully.</response>
        /// <response code="500">An error occurred while retrieving the coupons.</response>
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
                _logger.LogError(ex, "getAllCoupons method: An error occurred while retrieving Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Coupons"
                });
            }
        }

        /// <summary>
        /// Gets active coupons.
        /// </summary>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
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
                _logger.LogError(ex, "getActiveCoupons method: An error occurred while fetching active coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching active coupons"
                });
            }
        }

        /// <summary>
        /// Gets inactive coupons.
        /// </summary>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
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
                _logger.LogError(ex, "getInactiveCoupons method: An error occurred while fetching inactive coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching inactive coupons"
                });
            }
        }

        /// <summary>
        /// Gets active coupons by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
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
                _logger.LogError(ex, "getActiveCouponsByCompany method: An error occurred while fetching active coupons by company");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching active coupons by company"
                });
            }
        }

        /// <summary>
        /// Gets inactive coupons by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
        [HttpGet("getInactiveCouponsByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetInactiveCouponsByCompany(Guid companyID)
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
                _logger.LogError(ex, "getInactiveCouponsByCompany method: An error occurred while fetching inactive coupons by company");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching inactive coupons by company"
                });
            }
        }

        /// <summary>
        /// Gets active coupons by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
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
                _logger.LogError(ex, "getActiveCouponsByGenre method: An error occurred while fetching active coupons by genre");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching active coupons by genre"
                });
            }
        }

        /// <summary>
        /// Gets inactive coupons by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
        [HttpGet("getInactiveCouponsByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetInactiveCouponsByGenre(Guid genreID)
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
                _logger.LogError(ex, "getInactiveCouponsByGenre method: An error occurred while fetching inactive coupons by genre");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching inactive coupons by genre"
                });
            }
        }

        /// <summary>
        /// Gets coupons by title.
        /// </summary>
        /// <param name="title">The title of the coupons.</param>
        /// <returns>
        /// <response code="200">Status Code: 200 OK - Coupons found successfully.</response>
        /// <response code="500">Status Code: 500 Internal Server Error - An error occurred while fetching coupons.</response>
        /// </returns>
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
                _logger.LogError(ex, "getCouponsByTitle method: An error occurred while fetching coupons by title");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching coupons by title"
                });
            }
        }

    }
}