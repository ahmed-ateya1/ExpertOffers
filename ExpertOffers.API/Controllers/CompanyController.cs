using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.CompanyDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="CompanyController"/> class.
        /// </summary>
        /// <param name="companyServices">Service to handle company operations.</param>
        /// <param name="logger">Logger instance to record logs and errors.</param>
        public CompanyController(ICompanyServices companyServices, ILogger<CompanyController> logger)
        {
            _companyServices = companyServices;
            _logger = logger;
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
        /// <response code="404">Company not found.</response>
        /// <response code="500">An error occurred while updating the company.</response>
        [HttpPut("updateCompany")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> UpdateCompany([FromForm] CompanyUpdateRequest companyUpdate)
        {
            try
            {
                var company = await _companyServices.UpdateAsync(companyUpdate);
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
    }
}
