using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.ClientDto;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Handles client-related operations such as updating client information and fetching client details.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "USER")]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        private readonly ILogger<ClientController> _logger;
        // <summary>
        /// Initializes a new instance of the <see cref="ClientController"/> class.
        /// </summary>
        /// <param name="clientServices">Service for managing client-related operations.</param>
        /// <param name="logger">Logger instance for logging information and errors.</param>
        public ClientController(IClientServices clientServices, ILogger<ClientController> logger)
        {
            _clientServices = clientServices;
            _logger = logger;
        }
        /// <summary>
        /// Updates a client's information.
        /// </summary>
        /// <param name="clientDto">Client data to update.</param>
        /// <returns>
        /// An <see cref="ApiResponse"/> containing the status of the update operation.
        /// If successful, the response will include the updated client data.
        /// </returns>
        /// <response code="200">Client updated successfully.</response>
        /// <response code="404">Client not found.</response>
        /// <response code="500">Internal server error during the update operation.</response>
        [HttpPut("updateClient")]
        public async Task<ActionResult<ApiResponse>> UpdateClient(ClientUpdateRequest clientDto)
        {
            try
            {
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
        /// <param name="id">The unique identifier (ID) of the client to retrieve.</param>
        /// <returns>
        /// An <see cref="ApiResponse"/> containing the client information.
        /// If the client is not found, the response will include a not found status.
        /// </returns>
        /// <response code="200">Client fetched successfully.</response>
        /// <response code="404">Client not found.</response>
        /// <response code="500">Internal server error during the fetch operation.</response>
        [HttpGet("getClient/{id}")]
        public async Task<ActionResult<ApiResponse>> getClient(Guid id)
        {
            try
            {
                var client = await _clientServices.GetByAsync(x => x.ClientID == id);
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
