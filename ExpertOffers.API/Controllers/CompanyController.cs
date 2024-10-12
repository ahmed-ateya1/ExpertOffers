using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.CompanyDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller to manage company-related operations such as fetching, updating, and searching for companies.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]

    public class CompanyController : ControllerBase
    {
        private readonly ICompanyServices _companyServices;
        private readonly ILogger<CompanyController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyController"/> class.
        /// </summary>
        /// <param name="companyServices">Service to handle company operations.</param>
        /// <param name="logger">Logger instance to record logs and errors.</param>
        /// <param name="unitOfWork">Unit of work instance to handle database operations.</param>
        /// <param name="httpContextAccessor">HTTP context accessor instance to access HTTP context data.</param>
        public CompanyController(ICompanyServices companyServices, ILogger<CompanyController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _companyServices = companyServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }

        /// <summary>
        /// Fetches a list of all companies.
        /// </summary>
        /// <returns>An <see cref="ApiResponse"/> containing the list of companies if successful.</returns>
        /// <response code="200">Companies fetched successfully.</response>
        /// <response code="500">An error occurred while fetching the companies.</response>
        [HttpGet("getCompanies")]
        public async Task<ActionResult<ApiResponse>> GetCompanies()
        {
            try
            {
                var companies = await _companyServices.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Companies fetched successfully",
                    Result = companies,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompanies method: An error occurred while fetching companies.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching companies."
                });
            }
        }

        /// <summary>
        /// Fetches details of a specific company by ID.
        /// </summary>
        /// <param name="id">The ID of the company to fetch.</param>
        /// <returns>An <see cref="ApiResponse"/> containing the company details if found.</returns>
        /// <response code="200">Company fetched successfully.</response>
        /// <response code="404">Company not found.</response>
        /// <response code="500">An error occurred while fetching the company.</response>
        [HttpGet("getCompany/{id}")]
        public async Task<ActionResult<ApiResponse>> GetCompany(Guid id)
        {
            try
            {
                var company = await _companyServices.GetByAsync(x => x.CompanyID == id);
                if (company == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Company not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Company fetched successfully",
                    Result = company,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompany method: An error occurred while fetching the company.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching the company."
                });
            }
        }

        /// <summary>
        /// Searches for companies by name.
        /// </summary>
        /// <param name="companyName">The name of the company to search for.</param>
        /// <returns>An <see cref="ApiResponse"/> containing the list of companies matching the search criteria.</returns>
        /// <response code="200">Companies fetched successfully.</response>
        /// <response code="500">An error occurred while fetching companies.</response>
        [HttpGet("getCompanies/{companyName}")]
        public async Task<ActionResult<ApiResponse>> SearchCompanies(string companyName)
        {
            try
            {
                var companies = await _companyServices.GetAllAsync(x => x.CompanyName.ToUpper().Contains(companyName.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Companies fetched successfully",
                    Result = companies,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "SearchCompanies method: An error occurred while searching for companies.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while searching for companies."
                });
            }
        }

        /// <summary>
        /// Updates a company's details.
        /// "COMPANY" role is required to access this endpoint
        /// </summary>
        /// <param name="companyUpdate">The company update request object containing updated details.</param>
        /// <returns>An <see cref="ApiResponse"/> indicating the result of the update operation.</returns>
        /// <response code="200">Company updated successfully.</response>
        /// <response code="401">User unauthorized</response>
        /// <response code="404">Company not found.</response>
        /// <response code="500">An error occurred while updating the company.</response>
        [HttpPut("updateCompany")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> UpdateCompany([FromForm] CompanyUpdateRequest companyUpdate)
        {
            try
            {
                var email =  _httpContextAccessor.HttpContext
                .User.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User is not authenticated",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }
                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(u => u.Email == email);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User is not authenticated",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var company = await _unitOfWork.Repository<Company>()
                    .GetByAsync(c => c.UserID == user.Id);
                if (company == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Company not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Company updated successfully",
                    Result = company,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCompany method: An error occurred while updating the company.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating the company."
                });
            }
        }
        /// <summary>
        /// Updates a company's details.
        /// "COMPANY" role is required to access this endpoint
        /// </summary>
        /// <param name="id">The unique identifier for industrial</param>
        /// <returns>An <see cref="ApiResponse"/> indicating the result of the update operation.</returns>
        /// <response code="200">Company updated successfully.</response>
        /// <response code="404">industrial not found.</response>
        /// <response code="500">An error occurred while fetching the companies.</response>
        [HttpGet("getCompanyByIndustrial/{id:guid}")]
        public async Task<ActionResult<ApiResponse>> GetCompanyByIndustrial(Guid id)
        {
            try
            {
                var indstrial = await _unitOfWork.Repository<Industrial>()
                    .GetByAsync(x => x.IndustrialID == id);
                if (indstrial == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Industrial not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                var companies = await _companyServices.GetAllAsync(x => x.IndustrialID == id);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Companies fetched successfully",
                    Result = companies,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCompanyByIndustrial method: An error occurred while get the companies.");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating the company."
                });
            }
        }


        /// <summary>
        /// Retrieves companies by country ID.
        /// </summary>
        /// <response code="200">Companies fetched successfully.</response>
        /// <response code="400">Invalid country ID.</response>
        /// <response code="401">if user not authenticated.</response>
        /// <response code="404">Country not found.</response>
        /// <response code="500">An error occurred while fetching companies.</response>
        /// <returns>An API response with the list of companies for the specified country.</returns>
        [HttpGet("getCompanyForCurrentUserByCountry")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> getCompanyByCountry()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User is not authenticated",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }
                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(x => x.Email == email,includeProperties: "Country,City");
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                var companies = await _companyServices.
                    GetAllAsync(x => x.User.CountryID == user.CountryID);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Companies fetched successfully",
                    Result = companies,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getCompanyByCountry method: An error occurred while fetching Companies");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Companies"
                });
            }
        }


        /// <summary>
        /// Retrieves companies by country ID.
        /// User Must Be Authenticated for use this endpoint
        /// </summary>
        /// <response code="200">Companies fetched successfully.</response>
        /// <response code="400">Invalid city ID.</response>
        /// <response code="401">if user not authenticated.</response>
        /// <response code="404">City not found.</response>
        /// <response code="500">An error occurred while fetching companies.</response>
        /// <returns>An API response with the list of companies for the specified country.</returns>
        [HttpGet("getCompanyByCity")]
        [Authorize]
        public async Task<ActionResult<ApiResponse>> getCompanyByCity()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User is not authenticated",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }
                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(x => x.Email == email, includeProperties: "Country,City");
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "City not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                var companies = await _companyServices.
                    GetAllAsync(x => x.User.CityID == user.CityID);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Companies fetched successfully",
                    Result = companies,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getCompanyByCountry method: An error occurred while fetching Companies");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Companies"
                });
            }
        }
    }
}
