using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.OfferDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

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
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        /// <summary>
        /// Initializes a new instance of the <see cref="OfferController"/> class.
        /// </summary>
        /// <param name="offerServices">Service for managing offers.</param>
        /// <param name="logger">Logger for capturing logs.</param>
        /// <param name="unitOfWork">Unit of work for managing transactions.</param>
        /// <param name="httpContextAccessor">Accessor for managing HTTP context.</param>
        public OfferController(IOfferServices offerServices, ILogger<OfferController> logger, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            _offerServices = offerServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// Creates a new offer.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="request">The request containing the offer details.</param>
        /// <response code="200">Offer created successfully.</response>
        /// <response code="500">An error occurred while creating the offer.</response>
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
        /// Creates a new offer by an admin.
        /// </summary>
        /// <param name="request">The request object containing the details of the offer to be created.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse"/> object, which indicates whether the offer was successfully created or if an error occurred.
        /// </returns>
        /// <remarks>
        /// This method requires the user to have the "ADMIN" role. If the offer creation is successful, a 200 OK response is returned with the created offer data.
        /// If an error occurs during the creation process, a 500 Internal Server Error is returned.
        /// </remarks>
        /// <response code="200">Offer was created successfully.</response>
        /// <response code="500">An error occurred while creating the offer.</response>
        [HttpPost("createOfferByAdmin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> CreateOfferByAdmin([FromForm] OfferAddRequest request)
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "createOfferByAdmin method: An error occurred while Create Offer");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while create Offer"
                });
            }
        }

        /// <summary>
        /// Update existing offer by an admin.
        /// </summary>
        /// <param name="request">The request object containing the details of the offer to be updated.</param>
        /// <returns>
        /// An <see cref="ActionResult"/> containing an <see cref="ApiResponse"/> object, which indicates whether the offer was successfully created or if an error occurred.
        /// </returns>
        /// <remarks>
        /// This method requires the user to have the "ADMIN" role. If the offer creation is successful, a 200 OK response is returned with the created offer data.
        /// If an error occurs during the creation process, a 500 Internal Server Error is returned.
        /// </remarks>
        /// <response code="200">Offer updated successfully.</response>
        /// <response code="404">offer not found</response>
        /// <response code="500">An error occurred while updateing the offer.</response>
        [HttpPut("updateOfferByAdmin")]
        [Authorize(Roles = "ADMIN")]
        public async Task<ActionResult<ApiResponse>> UpdateOfferByAdmin([FromForm] OfferUpdateRequest request)
        {
            try
            {
                var offer = await _unitOfWork.Repository<Offer>()
                    .GetByAsync(x => x.OfferID == request.OfferID);
                if (offer == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
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
                _logger.LogError(ex, "updateOfferByAdmin method: An error occurred while Update Offer");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while update Offer"
                });
            }
        }
        /// <summary>
        /// Updates an existing offer.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="request">The updated offer details.</param>
        /// <response code="200">Offer updated successfully.</response>
        /// <response code="404">Offer not found.</response>
        /// <response code="500">An error occurred while updating the offer.</response>
        /// <returns>An API response indicating the result of the offer update.</returns>
        [HttpPut("updateOffer")]
        [Authorize(Roles = "COMPANY")]

        public async Task<ActionResult<ApiResponse>> UpdateOffer([FromForm] OfferUpdateRequest request)
        {
            try
            {
                var offer = await _unitOfWork.Repository<Offer>()
                    .GetByAsync(x=>x.OfferID == request.OfferID);
                if (offer == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
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
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="offerID">The unique ID of the offer to be deleted.</param>
        /// <response code="200">Offer deleted successfully.</response>
        /// <response code="400">Offer ID is required.</response>
        /// <response code="404">Offer not found.</response>
        /// <response code="500">An error occurred while deleting the offer.</response>
        /// <returns>An API response indicating the result of the offer deletion.</returns>
        [HttpDelete("deleteOffer/{offerID}")]
        [Authorize(Roles = "COMPANY,ADMIN")]
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
                var offer = await _unitOfWork.Repository<Offer>()
                    .GetByAsync(x => x.OfferID == offerID);
                if (offer == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer not found",
                        StatusCode = HttpStatusCode.NotFound
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="500">An error occurred while retrieving the offers.</response>
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Offer name is required.</response>
        /// <response code="500">An error occurred while retrieving the offers.</response>
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
        /// <response code="200">Offer retrieved successfully.</response>
        /// <response code="400">Offer ID is required.</response>
        /// <response code="404">Offer not found.</response>
        /// <response code="500">An error occurred while retrieving the offer.</response>
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
                var offer = await _unitOfWork.Repository<Offer>()
                    .GetByAsync(x => x.OfferID == offerID);
                if (offer == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Offer not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var response = await _offerServices.GetByAsync(x => x.OfferID == offerID);
              
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
        /// Retrieves offers by genre ID.
        /// </summary>
        /// <param name="genreID">The unique ID of the genre.</param>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Genre ID is required.</response>
        /// <response code="404">Genre not found.</response>
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
            var genre = await _unitOfWork.Repository<GenreOffer>()
                .GetByAsync(x => x.GenreID == genreID);
            if (genre == null)
            {
                return NotFound(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Genre not found",
                    StatusCode = HttpStatusCode.NotFound
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
        /// Retrieves all offers created by the authenticated company.
        /// </summary>
        /// <remarks>
        /// This endpoint is used by a company to fetch all the offers they have created. 
        /// The user must be authenticated and have the "COMPANY" role. 
        /// The endpoint returns a list of offers associated with the company.
        /// </remarks>
        /// <returns>
        /// Returns an ActionResult containing an ApiResponse:
        /// - 200 OK: When offers are successfully retrieved.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// - 404 Not Found: If the user or company is not found.
        /// - 500 Internal Server Error: If an error occurred while processing the request.
        /// </returns>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="404">User or company not found.</response>
        /// <response code="500">An error occurred while retrieving offers.</response>
        [HttpGet("getOffersByCompany")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> GetOffersByCompany()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.
                    User.FindFirstValue(ClaimTypes.Email);
                if(email == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User not authenticated",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }
                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(x => x.Email == email);
                if (user == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "User not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var company = await _unitOfWork.Repository<Company>()
                    .GetByAsync(x => x.UserID == user.Id);
                if (company == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Company not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var response = await _offerServices.GetAllAsync(x => x.CompanyID == company.CompanyID);
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
                _logger.LogError(ex, "getOffersByCompany method: An error occurred while get Offers");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Offers"
                });
            }
        }
        /// <summary>
        /// Retrieves offers by company ID.
        /// </summary>
        /// <param name="companyID">The unique ID of the company.</param>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Company ID is required.</response>
        /// <response code="404">Company not found.</response>
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
            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(x => x.CompanyID == companyID);
            if (company == null)
            {
                return NotFound(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Company not found",
                    StatusCode = HttpStatusCode.NotFound
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="500">An error occurred while retrieving active offers.</response>
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="500">An error occurred while retrieving inactive offers.</response>
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Company ID is required.</response>
        /// <response code="404">Company not found.</response>
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
            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(x => x.CompanyID == companyID);
            if (company == null)
            {
                return NotFound(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Company not found",
                    StatusCode = HttpStatusCode.NotFound
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
        /// Retrieves all active offers created by the authenticated company.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a company to fetch only the active offers they have created. 
        /// The user must be authenticated and have the "COMPANY" role to access this endpoint. 
        /// Active offers are those where the "IsActive" flag is set to true.
        /// </remarks>
        /// <returns>
        /// Returns an ActionResult containing an ApiResponse:
        /// - 200 OK: If active offers are successfully retrieved.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// - 404 Not Found: If the user or company is not found.
        /// - 500 Internal Server Error: If an error occurred while processing the request.
        /// </returns>
        [HttpGet("getOffersByCompanyActiveOnly")]
        [Authorize(Roles="COMPANY")]
        public async Task<ActionResult<ApiResponse>> GetOffersByCompanyActiveOnly()
        {
            try
            {
                var email = _httpContextAccessor.HttpContext.
                    User.FindFirstValue(ClaimTypes.Email);
                if (email == null)
                {
                    return Unauthorized(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "User not authenticated",
                        StatusCode = HttpStatusCode.Unauthorized
                    });
                }
                var user = await _unitOfWork.Repository<ApplicationUser>()
                    .GetByAsync(x => x.Email == email);
                if (user == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "User not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var company = await _unitOfWork.Repository<Company>()
                    .GetByAsync(x => x.UserID == user.Id);
                if (company == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Company not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var response = await _offerServices.GetAllAsync(x => x.CompanyID == company.CompanyID&&x.IsActive==true);
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
                _logger.LogError(ex, "getOffersByCompanyActiveOnly method: An error occurred while get Offers");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Offers"
                });
            }
        }
        /// <summary>
        /// Retrieves inactive offers by company ID.
        /// </summary>
        /// <param name="companyID">The unique ID of the company.</param>
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Company ID is required.</response>
        /// <response code="404">Company not found.</response>
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
            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(x => x.CompanyID == companyID);
            if (company == null)
            {
                return NotFound(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Company not found",
                    StatusCode = HttpStatusCode.NotFound
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Genre ID is required.</response>
        /// <response code="404">Genre not found.</response>
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
            var genre = await _unitOfWork.Repository<GenreOffer>()
                .GetByAsync(x => x.GenreID == genreID);
            if (genre == null)
            {
                return NotFound(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Genre not found",
                    StatusCode = HttpStatusCode.NotFound
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
        /// <response code="200">Offers retrieved successfully.</response>
        /// <response code="400">Genre ID is required.</response>
        /// <response code="404">Genre not found.</response>
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
            var genre = await _unitOfWork.Repository<GenreOffer>()
               .GetByAsync(x => x.GenreID == genreID);
            if (genre == null)
            {
                return NotFound(new ApiResponse()
                {
                    IsSuccess = false,
                    Messages = "Genre not found",
                    StatusCode = HttpStatusCode.NotFound
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
