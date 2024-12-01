using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BranchDto;
using ExpertOffers.Core.DTOS;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Controller for managing branch operations.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class BranchController : ControllerBase
    {
        private readonly IBranchServices _branchServices;
        private readonly ILogger<BranchController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        /// <summary>
        /// Initializes a new instance of the <see cref="BranchController"/> class.
        /// </summary>
        /// <param name="branchServices">The branch services.</param>
        /// <param name="logger">The logger.</param>
        public BranchController(IBranchServices branchServices, ILogger<BranchController> logger, IUnitOfWork unitOfWork)
        {
            _branchServices = branchServices;
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Adds a new branch.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="branchAddRequest">The branch add request containing the details of the branch to be added.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        /// <response code="200">Branch added successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPost("addBranch")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> AddBranch(BranchAddRequest branchAddRequest)
        {
            try
            {
                var company = await _unitOfWork.Repository<Company>().GetByAsync(c => c.CompanyID == branchAddRequest.CompanyID);
                if (company == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Company not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var response = await _branchServices.CreateAsync(branchAddRequest);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branch is added successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "AddBranch method: An error occurred while adding Branch");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while adding Branch"
                });
            }
        }

        /// <summary>
        /// Updates an existing branch.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="branchUpdateRequest">The branch update request containing the details to update the branch.</param>
        /// <returns>An ApiResponse indicating the result of the operation.</returns>
        /// <response code="200">Branch updated successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpPut("updateBranch")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> UpdateBranch(BranchUpdateRequest branchUpdateRequest)
        {
            try
            {
                var branch = await _branchServices.GetByAsync(b => b.BranchID == branchUpdateRequest.BranchID);
                if (branch == null)
                {
                    return NotFound(new ApiResponse
                    {
                        IsSuccess = false,
                        Messages = "Branch not found",
                        StatusCode = HttpStatusCode.NotFound
                    });
                }
                var response = await _branchServices.UpdateAsync(branchUpdateRequest);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branch is updated successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UpdateBranch method: An error occurred while updating Branch");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while updating Branch"
                });
            }
        }

        /// <summary>
        /// Deletes a branch by its ID.
        /// "COMPANY" role is required to access this endpoint.
        /// </summary>
        /// <param name="branchID">The ID of the branch to be deleted.</param>
        /// <returns>An IActionResult indicating the result of the operation.</returns>
        /// <response code="200">Branch deleted successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpDelete("deleteBranch/{branchID}")]
        [Authorize(Roles = "COMPANY")]
        public async Task<ActionResult<ApiResponse>> DeleteBranch(Guid branchID)
        {
            try
            {
                var response = await _branchServices.DeleteBranchAsync(branchID);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branch is deleted successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteBranch method: An error occurred while deleting Branch");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while deleting Branch"
                });
            }
        }

        /// <summary>
        /// Gets all branches for a specific company.
        /// </summary>
        /// <param name="companyID">The ID of the company whose branches are to be fetched.</param>
        /// <returns>An Api Response containing the list of branches.</returns>
        /// <response code="200">Branches fetched successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("getBranchesForCompany/{companyID}")]
        public async Task<ActionResult<ApiResponse>> GetBranchesForCompany(Guid companyID)
        {
            try
            {
                var response = await _branchServices.GetAllAsync(b => b.CompanyID == companyID);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branches are fetched successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBranchesForCompany method: An error occurred while fetching Branches");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Branches"
                });
            }
        }

        /// <summary>
        /// Gets a branch by its ID.
        /// </summary>
        /// <param name="branchID">The ID of the branch to be fetched.</param>
        /// <returns>An IActionResult containing the branch details.</returns>
        /// <response code="200">Branch fetched successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("getBranch/{branchID}")]
        public async Task<IActionResult> GetBranch(Guid branchID)
        {
            try
            {
                var response = await _branchServices.GetByAsync(b => b.BranchID == branchID);

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branch is fetched successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBranch method: An error occurred while fetching Branch");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Branch"
                });
            }
        }

        /// <summary>
        /// Gets all branches.
        /// </summary>
        /// <returns>An IActionResult containing the list of all branches.</returns>
        /// <response code="200">Branches fetched successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("getBranches")]
        public async Task<ActionResult<ApiResponse>> GetBranches()
        {
            try
            {
                var response = await _branchServices.GetAllAsync();

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branches are fetched successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBranches method: An error occurred while fetching Branches");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Branches"
                });
            }
        }

        /// <summary>
        /// Gets branches by name.
        /// </summary>
        /// <param name="branchName">The name of the branches to search for.</param>
        /// <returns>An IActionResult containing the list of matching branches.</returns>
        /// <response code="200">Branches fetched successfully.</response>
        /// <response code="500">Internal server error.</response>
        [HttpGet("getBranches/{branchName}")]
        public async Task<ActionResult<ApiResponse>> GetBranches(string branchName)
        {
            try
            {
                var response = await _branchServices.GetAllAsync(b => b.BranchName.Contains(branchName));

                return Ok(new ApiResponse
                {
                    IsSuccess = true,
                    Messages = "Branches are fetched successfully",
                    Result = response,
                    StatusCode = HttpStatusCode.OK
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetBranches method: An error occurred while fetching Branches");
                return StatusCode((int)HttpStatusCode.InternalServerError, new ApiResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    IsSuccess = false,
                    Messages = "An error occurred while fetching Branches"
                });
            }
        }
    }
}
