using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.BulletinDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using ExpertOffers.Infrastructure.Migrations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Security.Claims;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing bulletin operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BulletinController : ControllerBase
    {
        private readonly IBulletinServices _bulletinService;
        private readonly ILogger<BulletinController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletinController"/> class.
        /// </summary>
        /// <param name="bulletinService">The bulletin service.</param>
        /// <param name="logger">The logger.</param>
        /// <param name="unitOfWork">The unit of work.</param>
        /// <param name="httpContextAccessor">The HTTP context accessor.</param>
        public BulletinController(IBulletinServices bulletinService,
            ILogger<BulletinController> logger,
            IUnitOfWork unitOfWork,
            IHttpContextAccessor httpContextAccessor)
        {
            _bulletinService = bulletinService;
            _logger = logger;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
        }
        /// <summary>
        /// Creates a new bulletin.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="request">The bulletin add request.</param>
        /// <returns>An ApiResponse containing the created bulletin.</returns>
        /// <response code="200">Bulletin created successfully.</response>
        /// <response code="500">An error occurred while creating the bulletin.</response>
        [Authorize(Roles = "COMPANY")]
        [HttpPost("createBulletin")]
        public async Task<ActionResult<ApiResponse>> CreateBulletin([FromForm] BulletinAddRquest request)
        {
            try
            {
                var response = await _bulletinService.CreateAsync(request);
                if (response == null)
                {
                    return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        IsSuccess = false,
                        Messages = "An error occurred while creating Bulletin"
                    });
                }
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Bulletin created successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "createBulletin method: An error occurred while creating the bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while creating Bulletin"
                });
            }
        }

        /// <summary>
        /// Updates an existing bulletin.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="request">The bulletin update request.</param>
        /// <returns>An ApiResponse containing the updated bulletin.</returns>
        /// <response code="200">Bulletin updated successfully.</response>
        /// <response code="404">Bulletin not foun.</response>
        /// <response code="500">An error occurred while updating the bulletin.</response>
        [Authorize(Roles = "COMPANY")]
        [HttpPut("updateBulletin")]
        public async Task<ActionResult<ApiResponse>> UpdateBulletin([FromForm] BulletinUpdateRequest request)
        {
            try
            {
                var bulletin = await _unitOfWork.Repository<Bulletin>()
                   .GetByAsync(x => x.BulletinID == request.BulletinID);

                if (bulletin == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var result = await _bulletinService.UpdateAsync(request);
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletin updated successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateBulletin method: An error occurred while updating the bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating the bulletin"
                });
            }
        }

        /// <summary>
        /// Deletes a bulletin by its ID.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="id">The ID of the bulletin to delete.</param>
        /// <returns>An ApiResponse indicating the result of the deletion.</returns>
        /// <response code="200">Bulletin deleted successfully.</response>
        /// <response code="404">Bulletin not found.</response>
        /// <response code="500">An error occurred while deleting the bulletin.</response>
        [Authorize(Roles = "COMPANY")]
        [HttpDelete("deleteBulletin/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteBulletin(Guid id)
        {
            try
            {

                var bulletin = await _unitOfWork.Repository<Bulletin>()
                    .GetByAsync(x => x.BulletinID == id);

                if (bulletin == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var result = await _bulletinService.DeleteAsync(id);
                if (!result)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletin deleted successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteBulletin method: An error occurred while deleting the bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting the bulletin"
                });
            }
        }

        /// <summary>
        /// Gets a bulletin by its ID.
        /// </summary>
        /// <param name="id">The ID of the bulletin.</param>
        /// <returns>An ApiResponse containing the bulletin with the specified ID.</returns>
        /// <response code="200">Bulletin found.</response>
        /// <response code="404">Bulletin not found.</response>
        /// <response code="500">An error occurred while retrieving the bulletin.</response>
        [HttpGet("getBulletinById/{id}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinById(Guid id)
        {
            try
            {
                var bulletin = await _unitOfWork.Repository<Bulletin>()
                    .GetByAsync(x => x.BulletinID == id);

                if (bulletin == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var result = await _bulletinService.GetByAsync(x => x.BulletinID == id);
                if (result == null)
                {
                    return NotFound(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletin found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinById method: An error occurred while getting the bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving the bulletin"
                });
            }
        }

        /// <summary>
        /// Gets all bulletins.
        /// </summary>
        /// <returns>All bulletins.</returns>
        /// <response code="200">Returns all bulletins.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletins")]
        public async Task<ActionResult<ApiResponse>> GetBulletins()
        {
            try
            {
                var result = await _bulletinService.GetAllAsync();
               
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletins method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }

        /// <summary>
        /// Gets bulletins by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The bulletins with the specified genre ID.</returns>
        /// <response code="200">Returns bulletins matching the genre ID.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByGenre(Guid genreID)
        {
            try
            {
                var genre = await _unitOfWork.Repository<BulletinGenre>()
                                             .GetByAsync(x => x.GenreID == genreID);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Genre not found"
                    });
                }

                var result = await _bulletinService.GetAllAsync(x => x.GenreID == genreID);
                
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsByGenre method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }
        /// <summary>
        /// Retrieves all bulletins created by the authenticated company.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a company to fetch all bulletins they have created. 
        /// The user must be authenticated and associated with a company. 
        /// The endpoint returns a list of bulletins belonging to the company.
        /// </remarks>
        /// <returns>
        /// Returns an ActionResult containing an ApiResponse:
        /// - 200 OK: When bulletins are successfully retrieved.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// - 404 Not Found: If the user or company is not found.
        /// - 500 Internal Server Error: If an error occurred while processing the request.
        /// </returns>
        /// <response code="200">Bulletins retrieved successfully.</response>
        /// <response code="401">User not authenticated.</response>
        /// <response code="404">User or company not found.</response>
        /// <response code="500">An error occurred while retrieving bulletins.</response>
        [HttpGet("getBulletinsByCompany")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByCompany()
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
                var result = await _bulletinService.GetAllAsync(x => x.CompanyID == company.CompanyID);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Bulletins found successfully",
                    Result = result
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBulletinsByCompany method: An error occurred while retrieving Coupons");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while retrieving Bulletins"
                });
            }
        }
        /// <summary>
        /// Gets bulletins by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The bulletins with the specified company ID.</returns>
        /// <response code="200">Returns bulletins matching the company ID.</response>
        /// <response code="404">Returns when company NotFound</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByCompany(Guid companyID)
        {
            try
            {
                var company = await _unitOfWork.Repository<Company>()
                  .GetByAsync(x => x.CompanyID == companyID);
                if (company == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Company not found"
                    });
                }
                var result = await _bulletinService.GetAllAsync(x => x.CompanyID == companyID);
                
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsByCompany method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }

        /// <summary>
        /// Gets active bulletins.
        /// </summary>
        /// <returns>The active bulletins.</returns>
        /// <response code="200">Returns active bulletins.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsActive")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActive()
        {
            try
            {
                var result = await _bulletinService.GetAllAsync(x => x.IsActive == true);
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsActive method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }



        /// <summary>
        /// Gets inactive bulletins.
        /// </summary>
        /// <returns>The inactive bulletins.</returns>
        /// <response code="200">Returns inactive bulletins.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsInActive")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsInActive()
        {
            try
            {
                var result = await _bulletinService.GetAllAsync(x => x.IsActive == false);
               
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsInActive method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }

        /// <summary>
        /// Gets active bulletins by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The active bulletins with the specified genre ID.</returns>
        /// <response code="200">Returns active bulletins matching the genre ID.</response>
        /// <response code="404">Returns when genre not found</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsActiveByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActiveByGenre(Guid genreID)
        {
            try
            {
                var genre = await _unitOfWork.Repository<BulletinGenre>()
                    .GetByAsync(x => x.GenreID == genreID);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Genre not found"
                    });
                }
                var result = await _bulletinService
                .GetAllAsync(x => x.IsActive == true && x.GenreID == genreID);
                
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsActiveByGenre method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }

        /// <summary>
        /// Gets inactive bulletins by genre ID.
        /// </summary>
        /// <param name="genreID">The ID of the genre.</param>
        /// <returns>The inactive bulletins with the specified genre ID.</returns>
        /// <response code="200">Returns inactive bulletins matching the genre ID.</response>
        /// <response code="404">Returns when genre not found</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsInActiveByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsInActiveByGenre(Guid genreID)
        {
            try
            {
                var genre = await _unitOfWork.Repository<BulletinGenre>()
                   .GetByAsync(x => x.GenreID == genreID);
                if (genre == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Genre not found"
                    });
                }
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == false && x.GenreID == genreID);

                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsInActiveByGenre method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }


        /// <summary>
        /// Gets active bulletins by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The active bulletins with the specified company ID.</returns>
        /// <response code="200">Returns active bulletins matching the company ID.</response>
        /// <response code="404">Returns when country not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsActiveByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActiveByCompany(Guid companyID)
        {
            try
            {
                var company = await _unitOfWork.Repository<Company>()
                   .GetByAsync(x => x.CompanyID == companyID);
                if (company == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Company not found"
                    });
                }
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == true && x.CompanyID == companyID);

                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsActiveByCompany method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }
        /// <summary>
        /// Retrieves all active bulletins created by the authenticated company.
        /// </summary>
        /// <remarks>
        /// This endpoint allows a company to fetch only the active bulletins they have created. 
        /// The user must be authenticated and have the "COMPANY" role to access this endpoint. 
        /// Active coupons are those where the "IsActive" flag is set to true.
        /// </remarks>
        /// <returns>
        /// Returns an ActionResult containing an ApiResponse:
        /// - 200 OK: If active bulletins are successfully retrieved.
        /// - 401 Unauthorized: If the user is not authenticated.
        /// - 404 Not Found: If the user or company is not found.
        /// - 500 Internal Server Error: If an error occurred while processing the request.
        /// </returns>
        [HttpGet("getBulletinsActiveByCompany")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActiveByCompany()
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
                var response = await _bulletinService.GetAllAsync(x => x.CompanyID == company.CompanyID && x.IsActive == true);
                return Ok(new ApiResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    IsSuccess = true,
                    Messages = "Bulletins retrieved successfully",
                    Result = response
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsActiveByCompany method: An error occurred while get Bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while get Bulletins"
                });
            }
        }
        /// <summary>
        /// Gets inactive bulletins by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The inactive bulletins with the specified company ID.</returns>
        /// <response code="200">Returns inactive bulletins matching the company ID.</response>
        /// <response code="404">Returns when country not found.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsInActiveByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsInActiveByCompany(Guid companyID)
        {
            try
            {
                var company = await _unitOfWork.Repository<Company>()
                   .GetByAsync(x => x.CompanyID == companyID);
                if (company == null)
                {
                    return NotFound(new ApiResponse
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        IsSuccess = false,
                        Messages = "Company not found"
                    });
                }
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == false && x.CompanyID == companyID);
                
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsInActiveByCompany method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }


        /// <summary>
        /// Gets bulletins by title.
        /// </summary>
        /// <param name="title">The title to search for.</param>
        /// <returns>The bulletins with the specified title.</returns>
        /// <response code="200">Returns bulletins matching the title.</response>
        /// <response code="500">If an error occurs while processing the request.</response>
        [HttpGet("getBulletinsByTitle/{title}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByName(string title)
        {
            try
            {
                var result = await _bulletinService
                    .GetAllAsync(x => x.BulletinTitle.ToUpper().Contains(title.ToUpper()));
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletins found",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "getBulletinsByName method: An error occurred while getting the bulletins");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while getting the bulletins"
                });
            }
        }

    }
}
