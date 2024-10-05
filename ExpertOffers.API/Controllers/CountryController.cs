using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.CountryDto;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for handling operations related to Countries.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryServices _countryServices;
        private readonly ILogger<CountryController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CountryController"/> class.
        /// </summary>
        /// <param name="countryServices">The country service used to perform operations on country entities.</param>
        /// <param name="logger">The logger used for logging operations and errors.</param>
        public CountryController(ICountryServices countryServices, ILogger<CountryController> logger)
        {
            _countryServices = countryServices;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new country to the system.
        /// </summary>
        /// <param name="countryAddRequest">The details of the country to be added.</param>
        /// <returns>Returns a response indicating whether the country was added successfully.</returns>
        [HttpPost("addCountry")]
        public async Task<ActionResult<ApiResponse>> AddCountry(CountryAddRequest countryAddRequest)
        {
            try
            {
                var country = await _countryServices.AddCountry(countryAddRequest);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Country is added successfully",
                    Result = country,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddCountry method: An error occurred while adding Country");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while adding Country"
                });
            }
        }

        /// <summary>
        /// Updates the details of an existing country.
        /// </summary>
        /// <param name="countryUpdateRequest">The updated details of the country.</param>
        /// <returns>Returns a response indicating whether the country was updated successfully.</returns>
        [HttpPut("updateCountry")]

        public async Task<ActionResult<ApiResponse>> UpdateCountry(CountryUpdateRequest countryUpdateRequest)
        {
            try
            {
                var country = await _countryServices.UpdateCountry(countryUpdateRequest);
                if (country == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Country not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Country is updated successfully",
                    Result = country,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCountry method: An error occurred while updating Country");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Country"
                });
            }
        }

        /// <summary>
        /// Deletes a country by its ID.
        /// </summary>
        /// <param name="countryID">The ID of the country to be deleted.</param>
        /// <returns>Returns a response indicating whether the country was deleted successfully.</returns>
        [HttpDelete("deleteCountry")]

        public async Task<ActionResult<ApiResponse>> DeleteCountry(Guid countryID)
        {
            try
            {
                var isDeleted = await _countryServices.DeleteCountry(countryID);
                if (!isDeleted)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Country not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Country is deleted successfully",
                    Result = isDeleted,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteCountry method: An error occurred while deleting Country");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting Country"
                });
            }
        }

        /// <summary>
        /// Retrieves all countries in the system.
        /// </summary>
        /// <returns>Returns a list of all countries.</returns>
        [HttpGet("getCountries")]
        public async Task<ActionResult<ApiResponse>> GetCountries()
        {
            try
            {
                var countries = await _countryServices.GetCountries();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Countries are fetched successfully",
                    Result = countries,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCountries method: An error occurred while fetching Countries");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Countries"
                });
            }
        }

        /// <summary>
        /// Retrieves a country by its ID.
        /// </summary>
        /// <param name="countryID">The ID of the country to be retrieved.</param>
        /// <returns>Returns the details of the country if found.</returns>
        [HttpGet("getCountry")]
        public async Task<ActionResult<ApiResponse>> GetCountry(Guid countryID)
        {
            try
            {
                var country = await _countryServices.GetCountry(x => x.CountryID == countryID);
                if (country == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Country not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Country is fetched successfully",
                    Result = country,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCountry method: An error occurred while fetching Country");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Country"
                });
            }
        }

        /// <summary>
        /// Retrieves a list of countries by matching a partial or full country name.
        /// </summary>
        /// <param name="countryName">The partial or full name of the country.</param>
        /// <returns>Returns a list of countries that match the given name.</returns>
        [HttpGet("getCountries/{countryName}")]
        public async Task<ActionResult<ApiResponse>> GetCountries(string countryName)
        {
            try
            {
                var countries = await _countryServices.GetCountries(x => x.CountryName.ToUpper().Contains(countryName.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Countries are fetched successfully",
                    Result = countries,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCountries method: An error occurred while fetching Countries");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Countries"
                });
            }
        }
    }
}
