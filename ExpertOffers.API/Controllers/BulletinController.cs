using ExpertOffers.Core.Dtos.BulletinDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.Services;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

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

        /// <summary>
        /// Initializes a new instance of the <see cref="BulletinController"/> class.
        /// </summary>
        /// <param name="bulletinService">The bulletin service.</param>
        /// <param name="logger">The logger.</param>
        public BulletinController(IBulletinServices bulletinService,
            ILogger<BulletinController> logger)
        {
            _bulletinService = bulletinService;
            _logger = logger;
        }

        /// <summary>
        /// Creates a new bulletin.
        /// </summary>
        /// <param name="request">The bulletin add request.</param>
        /// <returns>The created bulletin.</returns>
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
                        Messages = "An error occurred while create Bulletin"
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
                _logger.LogError(ex, "createBulletin method: An error occurred while Create Bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while create Bulletin"
                });
            }
        }

        /// <summary>
        /// Updates an existing bulletin.
        /// </summary>
        /// <param name="request">The bulletin update request.</param>
        /// <returns>The updated bulletin.</returns>
        [HttpPut("updateBulletin")]
        public async Task<ActionResult<ApiResponse>> UpdateBulletin([FromForm] BulletinUpdateRequest request)
        {
            try
            {
                var result = await _bulletinService.UpdateAsync(request);
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletin Updated Successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "updateBulletin method: An error occurred while Update Bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while create Bulletin"
                });
            }
        }

        /// <summary>
        /// Deletes a bulletin by its ID.
        /// </summary>
        /// <param name="id">The ID of the bulletin to delete.</param>
        /// <returns>A response indicating the success of the deletion.</returns>
        [HttpDelete("deleteBulletin/{id}")]
        public async Task<ActionResult<ApiResponse>> DeleteBulletin(Guid id)
        {
            try
            {
                var result = await _bulletinService.DeleteAsync(id);
                if (!result)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
                return Ok(new ApiResponse()
                {
                    IsSuccess = true,
                    Messages = "Bulletin Deleted Successfully",
                    Result = result,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "deleteBulletin method: An error occurred while Delete Bulletin");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while create Bulletin"
                });
            }
        }

        /// <summary>
        /// Gets a bulletin by its ID.
        /// </summary>
        /// <param name="id">The ID of the bulletin.</param>
        /// <returns>The bulletin with the specified ID.</returns>
        [HttpGet("getBulletinById/{id}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinById(Guid id)
        {
            try
            {
                var result = await _bulletinService.GetByAsync(x => x.BulletinID == id);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletin not found",
                        StatusCode = HttpStatusCode.BadRequest
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
                    Messages = "An error occurred while getting the bulletin"
                });
            }
        }

        /// <summary>
        /// Gets all bulletins.
        /// </summary>
        /// <returns>All bulletins.</returns>
        [HttpGet("getBulletins")]
        public async Task<ActionResult<ApiResponse>> GetBulletins()
        {
            try
            {
                var result = await _bulletinService.GetAllAsync();
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByGenre(Guid genreID)
        {
            try
            {
                var result = await _bulletinService.GetAllAsync(x => x.GenreID == genreID);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        /// Gets bulletins by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The bulletins with the specified company ID.</returns>
        [HttpGet("getBulletinsByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByCompany(Guid companyID)
        {
            try
            {
                var result = await _bulletinService.GetAllAsync(x => x.CompanyID == companyID);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsActive")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActive()
        {
            try
            {
                var result = await _bulletinService.GetAllAsync(x => x.IsActive == true);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsInActive")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsInActive()
        {
            try
            {
                var result = await _bulletinService.GetAllAsync(x => x.IsActive == false);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsActiveByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActiveByGenre(Guid genreID)
        {
            try
            {
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == true && x.GenreID == genreID);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsInActiveByGenre/{genreID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsInActiveByGenre(Guid genreID)
        {
            try
            {
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == false && x.GenreID == genreID);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsActiveByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsActiveByCompany(Guid companyID)
        {
            try
            {
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == true && x.CompanyID == companyID);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        /// Gets inactive bulletins by company ID.
        /// </summary>
        /// <param name="companyID">The ID of the company.</param>
        /// <returns>The inactive bulletins with the specified company ID.</returns>
        [HttpGet("getBulletinsInActiveByCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsInActiveByCompany(Guid companyID)
        {
            try
            {
                var result = await _bulletinService
                    .GetAllAsync(x => x.IsActive == false && x.CompanyID == companyID);
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
        [HttpGet("getBulletinsByTitle/{title}")]
        public async Task<ActionResult<ApiResponse>> GetBulletinsByName(string title)
        {
            try
            {
                var result = await _bulletinService
                    .GetAllAsync(x => x.BulletinTitle.ToUpper().Contains(title.ToUpper()));
                if (result == null)
                {
                    return BadRequest(new ApiResponse()
                    {
                        IsSuccess = false,
                        Messages = "Bulletins not found",
                        StatusCode = HttpStatusCode.BadRequest
                    });
                }
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
