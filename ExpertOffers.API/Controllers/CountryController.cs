using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.CountryDto;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CountryController : ControllerBase
    {
        private readonly ICountryServices _countryServices;
        private readonly ILogger<CountryController> _logger;

        public CountryController(ICountryServices countryServices, ILogger<CountryController> logger)
        {
            _countryServices = countryServices;
            _logger = logger;
        }
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
        [HttpDelete("deleteCountry")]
        public async Task<ActionResult<ApiResponse>> DeleteCountry(Guid countryID)
        {
            try
            {
                var isDeleted = await _countryServices.DeleteCountry(countryID);
                if (isDeleted == false)
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
                _logger.LogError(ex, "GetCountries method: An error occurred while fetched Countries");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetched Countries"
                });
            }
        }
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
                _logger.LogError(ex, "GetCountry method: An error occurred while fetched Country");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetched Country"
                });
            }
        }

        [HttpGet("getCountries/{countryName}")]
        public async Task<ActionResult<ApiResponse>> GetCountries(string countryName)
        {
            try
            {
                var countries = await _countryServices.GetCountries(x => x.CountryName.Contains(countryName.ToUpper()));
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
                _logger.LogError(ex, "GetCountries method: An error occurred while fetched Countries");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetched Countries"
                });
            }
        }
    }
}
