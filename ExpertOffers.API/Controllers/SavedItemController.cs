using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.SavedItemDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using ExpertOffers.Infrastructure.UnitOfWorkConfig;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ExpertOffers.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SavedItemController : ControllerBase
    {
        private readonly ISavedItemServices _savedItemServices;
        private readonly ILogger<SavedItemController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SavedItemController(ISavedItemServices savedItemServices
            , ILogger<SavedItemController> logger
            , IUnitOfWork unitOfWork
            ,IHttpContextAccessor httpContextAccessor)
        {
            _savedItemServices = savedItemServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        [Authorize(Roles = "USER")]
        [HttpPost("addToSaved")]
        public async Task<ActionResult<ApiResponse>> AddToSaved(SavedItemAddRequest savedItem)
        {
            try
            {

                var result = await _savedItemServices.CreateAsync(savedItem);
                return Ok(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Saved Item Added Successfully",
                    Result = result

                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "addToSaved method: An error occurred while addToSaved");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while add To Saved"
                });
            }
        }
        [Authorize(Roles = "USER")]
        [HttpDelete("deleteSavedItem/{savedItemID}")]
        public async Task<ActionResult<ApiResponse>> DeleteSavedItem(Guid savedItemID)
        {
            try
            {
                
                var result = await _savedItemServices.DeleteAsync(savedItemID);
                return Ok(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Saved Item Deleted Successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteSavedItem method: An error occurred while deleteSavedItem");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while delete Saved Item"
                });
            }
        }
        [Authorize(Roles = "USER")]
        [HttpGet("getSavedItems")]
        public async Task<ActionResult<ApiResponse>> GetSavedItems()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext
                 .User.FindFirstValue(ClaimTypes.Email)
                 ?? throw new UnauthorizedAccessException("User is not authenticated.");

                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(u => u.Email == email)
                    ?? throw new UnauthorizedAccessException("User is not authenticated.");

                var client = await _unitOfWork.Repository<Client>()
                    .GetByAsync(x => x.UserID == user.Id) 
                    ?? throw new Exception("Client not found");

                var result = await _savedItemServices.GetAllAsync(x=>x.ClientID == client.ClientID);
                return Ok(new ApiResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Saved Items Fetched Successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getSavedItems method: An error occurred while getSavedItems");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Saved Items"
                });
            }
        }
        



    }
}
