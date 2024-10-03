using ExpertOffers.Core.Dtos.IndustrialDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing industrial operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class IndustrialController : ControllerBase
    {
        private readonly IIndustrialServices _industrialServices;
        private readonly ILogger<IndustrialController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndustrialController"/> class.
        /// </summary>
        /// <param name="industrialServices">The industrial services.</param>
        /// <param name="logger">The logger.</param>
        public IndustrialController(IIndustrialServices industrialServices, ILogger<IndustrialController> logger)
        {
            _industrialServices = industrialServices;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new industrial entity.
        /// </summary>
        /// <param name="industrialAddRequest">The request object containing the industrial details to be added.</param>
        /// <returns>An <see cref="ActionResult"/> with the result of the creation.</returns>
        [HttpPost("createIndustrial")]
        public async Task<ActionResult<ApiResponse>> CreateIndustrial([FromBody] IndustrialAddRequest industrialAddRequest)
        {
            try
            {
                var industrial = await _industrialServices.CreateAsync(industrialAddRequest);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Industrial added successfully",
                    Result = industrial,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CreateIndustrial method: An error occurred while adding Industrial");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while adding Industrial"
                });
            }
        }

        /// <summary>
        /// Updates an existing industrial entity.
        /// </summary>
        /// <param name="industrialUpdateRequest">The request object containing the updated industrial details.</param>
        /// <returns>An <see cref="ActionResult"/> with the result of the update.</returns>
        [HttpPut("updateIndustrial")]
        public async Task<ActionResult<ApiResponse>> UpdateIndustrial([FromBody] IndustrialUpdateRequest industrialUpdateRequest)
        {
            try
            {
                var industrial = await _industrialServices.UpdateAsync(industrialUpdateRequest);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Industrial updated successfully",
                    Result = industrial,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateIndustrial method: An error occurred while updating Industrial");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Industrial"
                });
            }
        }

        /// <summary>
        /// Deletes an industrial entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the industrial entity to delete.</param>
        /// <returns>An <see cref="ActionResult"/> with the result of the deletion.</returns>
        [HttpDelete("deleteIndustrial/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteIndustrial(Guid id)
        {
            try
            {
                var industrial = await _industrialServices.DeleteAsync(id);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Industrial deleted successfully",
                    Result = industrial,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteIndustrial method: An error occurred while deleting Industrial");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting Industrial"
                });
            }
        }

        /// <summary>
        /// Retrieves an industrial entity by its ID.
        /// </summary>
        /// <param name="id">The ID of the industrial entity.</param>
        /// <returns>An <see cref="ActionResult"/> with the industrial entity details.</returns>
        [HttpGet("getIndustrial/{id}")]
        public async Task<ActionResult<ApiResponse>> GetIndustrial(Guid id)
        {
            try
            {
                var industrial = await _industrialServices.GetByAsync(x => x.IndustrialID == id);
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Industrial retrieved successfully",
                    Result = industrial,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetIndustrial method: An error occurred while retrieving Industrial");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Industrial"
                });
            }
        }

        /// <summary>
        /// Retrieves all industrial entities.
        /// </summary>
        /// <returns>An <see cref="ActionResult"/> with the list of all industrial entities.</returns>
        [HttpGet("getIndustrials")]
        public async Task<ActionResult<ApiResponse>> GetIndustrials()
        {
            try
            {
                var industrials = await _industrialServices.GetAllAsync();
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Industrials retrieved successfully",
                    Result = industrials,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetIndustrials method: An error occurred while retrieving Industrials");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Industrials"
                });
            }
        }

        /// <summary>
        /// Retrieves industrial entities by name.
        /// </summary>
        /// <param name="industrialName">The name of the industrial entity to search for.</param>
        /// <returns>An <see cref="ActionResult"/> with the list of matching industrial entities.</returns>
        [HttpGet("getIndustrials/{industrialName}")]
        public async Task<ActionResult<ApiResponse>> GetIndustrials(string industrialName)
        {
            try
            {
                var industrials = await _industrialServices.GetAllAsync(x => x.IndustrialName.ToUpper().Contains(industrialName.ToUpper()));
                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Industrials retrieved successfully",
                    Result = industrials,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetIndustrials method: An error occurred while retrieving Industrials");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Industrials"
                });
            }
        }
    }
}
