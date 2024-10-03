using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.CityDto;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing city operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CityController : ControllerBase
    {
        private readonly ICityServices _cityService;
        private readonly ILogger<CityController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="CityController"/> class.
        /// </summary>
        /// <param name="cityService">The city services.</param>
        /// <param name="logger">The logger.</param>
        public CityController(ICityServices cityService, ILogger<CityController> logger)
        {
            _cityService = cityService;
            _logger = logger;
        }

        /// <summary>
        /// Adds a new city.
        /// </summary>
        /// <param name="cityDto">The city add request containing the details of the city to be added.</param>
        /// <returns>An ActionResult containing the result of the operation.</returns>
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

        /// <summary>
        /// Updates an existing city.
        /// </summary>
        /// <param name="cityDto">The city update request containing the details to update the city.</param>
        /// <returns>An ActionResult containing the result of the operation.</returns>
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

        /// <summary>
        /// Deletes a city by its ID.
        /// </summary>
        /// <param name="cityID">The ID of the city to be deleted.</param>
        /// <returns>An ActionResult containing the result of the operation.</returns>
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

        /// <summary>
        /// Gets all cities for a specific country.
        /// </summary>
        /// <param name="countryID">The ID of the country whose cities are to be fetched.</param>
        /// <returns>An ActionResult containing the list of cities.</returns>
        [HttpGet("getCitiesForCountry/{countryID}")]
        public async Task<ActionResult<ApiResponse>> GetCitiesForCountry(Guid countryID)
        {
            try
            {
                var cities = await _cityService.GetAllAsync(x => x.CountryID == countryID);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Cities are fetched successfully",
                    Result = cities,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCitiesForCountry method: An error occurred while fetching Cities");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Cities"
                });
            }
        }

        /// <summary>
        /// Gets all cities.
        /// </summary>
        /// <returns>An ActionResult containing the list of all cities.</returns>
        [HttpGet("getCities")]
        public async Task<ActionResult<ApiResponse>> GetCities()
        {
            try
            {
                var cities = await _cityService.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Cities are fetched successfully",
                    Result = cities,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetCities method: An error occurred while fetching Cities");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Cities"
                });
            }
        }

        /// <summary>
        /// Gets cities by name.
        /// </summary>
        /// <param name="cityName">The name of the cities to search for.</param>
        /// <returns>An ActionResult containing the list of matching cities.</returns>
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
                _logger.LogError(ex, "GetCity method: An error occurred while fetching City");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching City"
                });
            }
        }
    }
}
