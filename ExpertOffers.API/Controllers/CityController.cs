using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.CityDto;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityServices _cityService;
        private readonly ILogger<CityController> _logger;

        public CityController(ICityServices cityService, ILogger<CityController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }
        [HttpPost("addCity")]
        public async Task<ActionResult<ApiResponse>> AddCity([FromBody] CityAddRequest cityDto)
        {
            try
            {
                var city = await _cityService.AddCityAsync(cityDto);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "City added successfully",
                    Result = city,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddCity method: An error occurred while adding City");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while adding City"
                });
            }
        }

        [HttpPut("updateCity")]
        public async Task<ActionResult<ApiResponse>> UpdateCity([FromBody] CityUpdateRequest cityDto)
        {
            try
            {
                var city = await _cityService.UpdateCityAsync(cityDto);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "City updated successfully",
                    Result = city,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateCity method: An error occurred while updating City");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating City"
                });
            }
        }

        [HttpDelete("deleteCity/{cityID}")]
        public async Task<ActionResult<ApiResponse>> DeleteCity(Guid cityID)
        {
            try
            {
                var result = await _cityService.DeleteCityAsync(cityID);
                if (result == false)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "City not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "City deleted successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteCity method: An error occurred while deleting City");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting City"
                });
            }
        }

        [HttpGet("getCitiesForCountry/{countryID}")]
        public async Task<ActionResult<ApiResponse>> GetCitiesForCountry(Guid countryID)
        {
            try
            {
                var cities = await _cityService.GetAllAsync(x => x.CountryID == countryID);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Countries are fetched successfully",
                    Result = cities,
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

        [HttpGet("getCities")]
        public async Task<ActionResult<ApiResponse>> GetCities()
        {
            try
            {
                var cities = await _cityService.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Countries are fetched successfully",
                    Result = cities,
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

        [HttpGet("getCities/{cityName}")]
        public async Task<ActionResult<ApiResponse>> GetCity(string cityName)
        {
            try
            {
                var city = await _cityService.GetAllAsync(x => x.CityName.Contains(cityName.ToUpper()));

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "City fetched successfully",
                    Result = city,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCity method: An error occurred while fetched City");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetched City"
                });
            }
        }
    }
}
