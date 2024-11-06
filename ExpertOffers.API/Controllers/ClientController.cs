using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.ClientDto;
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
    /// Handles client-related operations such as updating client information and fetching client details.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER,ADMIN")]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        private readonly ILogger<ClientController> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        // <summary>
        /// Initializes a new instance of the <see cref="ClientController"/> class.
        /// </summary>
        /// <param name="clientServices">Service for managing client-related operations.</param>
        /// <param name="logger">Logger instance for logging information and errors.</param>
        public ClientController(IClientServices clientServices, ILogger<ClientController> logger, IHttpContextAccessor httpContextAccessor, IUnitOfWork unitOfWork)
        {
            _clientServices = clientServices;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
            _unitOfWork = unitOfWork;
        }
        /// <summary>
        /// Updates the details of an existing client.
        /// </summary>
        /// <param name="clientDto">An object containing the updated client information.</param>
        /// <returns>
        /// An <see cref="ApiResponse"/> indicating the success or failure of the update operation.
        /// </returns>
        /// <response code="200">Returns an ApiResponse with success status and the updated client information.</response>
        /// <response code="404">Returns an ApiResponse indicating that the country, city, or client was not found.</response>
        /// <response code="500">Returns an ApiResponse indicating an internal server error occurred.</response>
        [HttpPut("updateClient")]
        [Authorize(Roles = "USER,ADMIN")]
        public async Task<ActionResult<ApiResponse>> UpdateClient(ClientUpdateRequest clientDto)
        {
            try
            {
                var county = await _unitOfWork.Repository<Country>()
                    .GetByAsync(x => x.CountryID == clientDto.CountryID);
                if (county == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Country not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var city = await _unitOfWork.Repository<City>()
                    .GetByAsync(x => x.CityID == clientDto.CityID);
                if (city == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "City not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var client = await _clientServices.UpdateAsync(clientDto);
                if (client == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Client not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Client updated successfully",
                    Result = client,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateClient method: An error occurred while updating Client");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Client"
                });
            }
        }
        /// <summary>
        /// Reterive all client information.
        /// </summary>
        /// <returns>
        /// An <see cref="ApiResponse"/> containing the client information.
        /// If the client is not found, the response will include a not found status.
        /// </returns>
        /// <response code="200">Client fetched successfully.</response>
        /// <response code="401">Unauthorized</response>
        /// <response code="404">Client not found.</response>
        /// <response code="500">Internal server error during the fetch operation.</response>
        [HttpGet("getClient")]
        [Authorize(Roles = "USER")]
        public async Task<ActionResult<ApiResponse>> getClient()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
                if (string.IsNullOrEmpty(email))
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Unauthorized",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }
                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(x => x.Email == email);
                if (user == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "NorFound",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }

                var client = await _clientServices.GetByAsync(x => x.UserID == user.Id);
                if (client == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Client not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Client fetched successfully",
                    Result = client,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getClient method: An error occurred while fetching Client");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Client"
                });
            }
        }

        
    }

}
