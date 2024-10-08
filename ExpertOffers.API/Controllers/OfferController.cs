using ExpertOffers.Core.Dtos.OfferDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{  
    /// <summary>
    /// Controller responsible for managing offers.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class OfferController : ControllerBase
    {
        private readonly IOfferServices _offerServices;
        private readonly ILogger<OfferController> _logger;
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferController"/> class.
        /// </summary>
        /// <param name="offerServices">Service for managing offers.</param>
        /// <param name="logger">Logger for capturing logs.</param>
        public OfferController(IOfferServices offerServices, ILogger<OfferController> logger)
        {
            _offerServices = offerServices;
            _logger = logger;
        }
        /// <summary>
        /// Creates a new offer.
        /// "COMPANY" role is required to access this endpoint
        /// </summary>
        /// <param name="request">The offer details to be created.</param>
        /// <returns>An API response indicating the result of the offer creation.</returns>
        [HttpPost("createOffer")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> CreateOffer([FromForm] OfferAddRequest request)
        {
            try
            {
                var response = await _offerServices.CreateAsync(request);
                if (response == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while create Offer"
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offer created successfully",
                    Result = response
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "createOffer method: An error occurred while Create Offer");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while create Offer"
                });
            }
        }
        /// <summary>
        /// Updates an existing offer.
        /// "COMPANY" role is required to access this endpoint
        /// </summary>
        /// <param name="request">The updated offer details.</param>
        /// <returns>An API response indicating the result of the offer update.</returns>
        [HttpPut("updateOffer")]
        [Authorize(Roles = "COMPANY")]

        public async Task<ActionResult<ApiResponse>> UpdateOffer([FromForm] OfferUpdateRequest request)
        {
            try
            {
                var response = await _offerServices.UpdateAsync(request);
                if (response == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while update Offer"
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offer updated successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateOffer method: An error occurred while Update Offer");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while update Offer"
                });
            }
        }
        /// <summary>
        /// Deletes an existing offer by its ID.
        /// "COMPANY" role is required to access this endpoint
        /// </summary>
        /// <param name="offerID">The unique ID of the offer to be deleted.</param>
        /// <returns>An API response indicating the result of the offer deletion.</returns>
        [HttpDelete("deleteOffer/{offerID}")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> DeleteOffer(Guid offerID)
        {
            try
            {
                if(offerID == Guid.Empty)
                {
                    return BadRequest( new ApiResponse(){
                       IsSuccess = false,
                       Messages = "Offer ID is required",
                       StatusCode = HttpStatusCode.BadRequest
                    });
                }
                var response = await _offerServices.DeleteAsync(offerID);
                if (!response)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while Delete Offer"
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offer deleted successfully"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteOffer method: An error occurred while Delete Offer");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while Delete Offer"
                });
            }
        }
        /// <summary>
        /// Retrieves all available offers.
        /// </summary>
        /// <returns>An API response with the list of offers.</returns>
        [HttpGet("getOffers")]
        public async Task<ActionResult<ApiResponse>> GetOffers()
        {
            try
            {
                var response = await _offerServices.GetAllAsync();
                
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getOffers method: An error occurred while get Offers");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Offers"
                });
            }
        }
        /// <summary>
        /// Retrieves offers based on the offer name.
        /// </summary>
        /// <param name="offerName">The name of the offer to search.</param>
        /// <returns>An API response with the matching offers.</returns>
        [HttpGet("getOffersBy/{offerName}")]
        public async Task<ActionResult<ApiResponse>> GetOffersBy(string offerName)
        {
            try
            {
                if (string.IsNullOrEmpty(offerName))
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer name is required",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                var response = await _offerServices.GetAllAsync(x => x.OfferTitle.ToUpper().Contains(offerName.ToUpper()));
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getOffersBy method: An error occurred while get Offers");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Offers"
                });
            }
        }
        /// <summary>
        /// Retrieves an offer by its ID.
        /// </summary>
        /// <param name="offerID">The unique ID of the offer.</param>
        /// <returns>An API response with the requested offer details.</returns>
        [HttpGet("getOffer/{offerID}")]
        public async Task<ActionResult<ApiResponse>> GetOffer(Guid offerID)
        {
            try
            {
                if (offerID == Guid.Empty)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer ID is required",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                var response = await _offerServices.GetByAsync(x => x.OfferID == offerID);
                if (response == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offer retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getOffer method: An error occurred while get Offer");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Offer"
                });
            }
        }
        /// <summary>
        /// Retrieves an offers by genre ID.
        /// </summary>
        /// <param name="genreID">The unique ID of the genreOffer.</param>
        /// <returns>An API response with the requested offer details.</returns>
        [HttpGet("getOffersByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetOffersByGenre(Guid genreID)
        {
            if(genreID == Guid.Empty)
            {
                return BadRequest(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "genreID ID is required",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            var genres = await _offerServices.GetAllAsync(x=>x.GenreID == genreID);
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Messages = "Offers retrieved successfully",
                Result = genres
            });
        }
        /// <summary>
        /// Retrieves an offers by company ID.
        /// </summary>
        /// <param name="companyID">The unique ID of the company.</param>
        /// <returns>An API response with the requested offer details.</returns>
        [HttpGet("getOffersByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetOffersByCompany(Guid companyID)
        {
            if (companyID == Guid.Empty)
            {
                return BadRequest(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Company ID is required",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            var companies = await _offerServices.GetAllAsync(x => x.CompanyID == companyID);
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Messages = "Offers retrieved successfully",
                Result = companies
            });
        }

        /// <summary>
        /// Retrieves active offers only.
        /// </summary>
        /// <returns>An API response with the list of active offers.</returns>
        [HttpGet("getOffersActiveOnly")]
        public async Task<ActionResult<ApiResponse>> GetOffersActiveOnly()
        {
            try
            {
                var response = await _offerServices.GetAllAsync(x => x.IsActive == true);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getOffersActiveOnly method: An error occurred while getting active offers");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting active offers"
                });
            }
        }
        /// <summary>
        /// Retrieves inactive offers only.
        /// </summary>
        /// <returns>An API response with the list of inactive offers.</returns>
        [HttpGet("getOffersInactiveOnly")]
        public async Task<ActionResult<ApiResponse>> GetOffersInactiveOnly()
        {
            try
            {
                var response = await _offerServices.GetAllAsync(x => x.IsActive == false);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Offers retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getOffersInactiveOnly method: An error occurred while getting inactive offers");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting inactive offers"
                });
            }
        }
        /// <summary>
        /// Retrieves active offers by company ID.
        /// </summary>
        /// <param name="companyID">The unique ID of the company.</param>
        /// <returns>An API response with the list of active offers for the specified company.</returns>
        [HttpGet("getOffersByCompanyActiveOnly/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetOffersByCompanyActiveOnly(Guid companyID)
        {
            if (companyID == Guid.Empty)
            {
                return BadRequest(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Company ID is required",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            var companies = await _offerServices.GetAllAsync(x => x.CompanyID == companyID && x.IsActive == true);
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Messages = "Offers retrieved successfully",
                Result = companies
            });
        }
        /// <summary>
        /// Retrieves inactive offers by company ID.
        /// </summary>
        /// <param name="companyID">The unique ID of the company.</param>
        /// <returns>An API response with the list of inactive offers for the specified company.</returns>
        [HttpGet("getOffersByCompanyInactiveOnly/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetOffersByCompanyInactiveOnly(Guid companyID)
        {
            if (companyID == Guid.Empty)
            {
                return BadRequest(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Company ID is required",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            var companies = await _offerServices.GetAllAsync(x => x.CompanyID == companyID && x.IsActive == false);
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Messages = "Offers retrieved successfully",
                Result = companies
            });
        }
        /// <summary>
        /// Retrieves active offers by genre ID.
        /// </summary>
        /// <param name="genreID">The unique ID of the genre.</param>
        /// <returns>An API response with the list of active offers for the specified genre.</returns>
        [HttpGet("getOffersByGenreActiveOnly/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetOffersByGenreActiveOnly(Guid genreID)
        {
            if (genreID == Guid.Empty)
            {
                return BadRequest(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Genre ID is required",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            var genres = await _offerServices.GetAllAsync(x => x.GenreID == genreID && x.IsActive == true);
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Messages = "Offers retrieved successfully",
                Result = genres
            });
        }
        /// <summary>
        /// Retrieves inactive offers by genre ID.
        /// </summary>
        /// <param name="genreID">The unique ID of the genre.</param>
        /// <returns>An API response with the list of inactive offers for the specified genre.</returns>
        [HttpGet("getOffersByGenreInactiveOnly/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetOffersByGenreInactiveOnly(Guid genreID)
        {
            if (genreID == Guid.Empty)
            {
                return BadRequest(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Genre ID is required",
                    StatusCode = HttpStatusCode.BadRequest
                });
            }
            var genres = await _offerServices.GetAllAsync(x => x.GenreID == genreID && x.IsActive == false);
            return Ok(new ApiResponse
            {
                StatusCode = HttpStatusCode.OK,
                IsSuccess = true,
                Messages = "Offers retrieved successfully",
                Result = genres
            });
        }

    }
}
