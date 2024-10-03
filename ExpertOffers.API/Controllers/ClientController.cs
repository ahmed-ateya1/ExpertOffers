using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.DTOS.ClientDto;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClientController : ControllerBase
    {
        private readonly IClientServices _clientServices;
        private readonly ILogger<ClientController> _logger;

        public ClientController(IClientServices clientServices, ILogger<ClientController> logger)
        {
            _clientServices = clientServices;
            _logger = logger;
        }

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

        [HttpDelete("deleteClient")]
        public async Task<ActionResult<ApiResponse>> DeleteClient()
        {
            try
            {
                var client = await _clientServices.DeleteAsync();
                if (!client)
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
                    Messages = "Client deleted successfully",
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteClient method: An error occurred while deleting Client");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting Client"
                });
            }
        }
    }

}
