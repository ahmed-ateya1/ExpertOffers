using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.DTOS.AuthenticationDTO;
using ExpertOffers.Core.DTOS.CityDto;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ExpertOffers.API.Controllers
{
    /// <summary>
    /// Manages user accounts, including registration, login, password reset, roles, and token handling.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccountController"/> class.
        /// </summary>
        /// <param name="authenticationServices">Authentication service.</param>
        /// <param name="userManager">User manager service.</param>
        /// <param name="emailSender">Email sender service.</param>
        /// <param name="signInManager">Sign-in manager service.</param>
        public AccountController(IAuthenticationServices authenticationServices, UserManager<ApplicationUser> userManager, IEmailSender emailSender , SignInManager<ApplicationUser> signInManager )
        {
            _authenticationServices = authenticationServices;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }
        /// <summary>
        /// Registers a new client account.
        /// </summary>
        /// <param name="registerDTO">Client registration details.</param>
        /// <returns>Authentication response with token and status.</returns>
        [HttpPost("register-Client")]
        public async Task<ActionResult<AuthenticationResponse>> RegisterCleintAsync([FromBody] ClientRegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("|", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return BadRequest(errors);
            }

            var result = await _authenticationServices.RegisterClientAsync(registerDTO);
            if (!result.IsAuthenticated)
                return Problem(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);
        }
        /// <summary>
        /// Registers a new company account.
        /// </summary>
        /// <param name="registerDTO">Company registration details.</param>
        /// <returns>Authentication response with token and status.</returns>
        [HttpPost("register-Comapny")]
        public async Task<ActionResult<AuthenticationResponse>> RegisterComapnyAsync([FromForm] CompanyRegisterDTO registerDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("|", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return BadRequest(errors);
            }

            var result = await _authenticationServices.RegisterCompanyAsync(registerDTO);
            if (!result.IsAuthenticated)
                return Problem(result.Message);

            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            }

            return Ok(result);
        }
        /// <summary>
        /// Logs in a user or company.
        /// </summary>
        /// <param name="loginDTO">Login credentials (email, password).</param>
        /// <returns>Authentication response with token and status.</returns>
        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> LoginAsync([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("|", ModelState.Values.SelectMany(x => x.Errors).Select(x => x.ErrorMessage));
                return BadRequest(errors);
            }

            var result = await _authenticationServices.LoginAsync(loginDTO);
            if (!result.IsAuthenticated)
                return Problem(result.Message);

            
            if (!string.IsNullOrEmpty(result.RefreshToken))
            {
                SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            }
            return Ok(result);
        }

        /// <summary>
        /// Sends a password reset link to the user's email.
        /// </summary>
        /// <param name="forgotPassword">Email to send the password reset link to.</param>
        /// <returns>Status message.</returns>

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDTO forgotPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(forgotPassword.Email!);

            if (user == null)
                return Ok("If the email is associated with an account, a reset password link will be sent.");

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);


            var param = new Dictionary<string, string?>
            {
                {"code", code},
                {"email", forgotPassword.Email}
            };
            var callback = QueryHelpers.AddQueryString(forgotPassword.ClientUri!, param);

            await _emailSender.SendEmailAsync(user.Email, "Reset Password",
                $"Please reset your password by <a href='{callback}'>clicking here</a>.");

            return Ok("If the email is associated with an account, a reset password link will be sent.");
        }
        /// <summary>
        /// Resets the user's password.
        /// </summary>
        /// <param name="resetPassword">Reset password request details.</param>
        /// <returns>Status message.</returns>
        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPassword)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            var user = await _userManager.FindByEmailAsync(resetPassword.Email);
            if (user == null)
                return BadRequest("Invalid Request");

            var result = await _userManager.ResetPasswordAsync(user, resetPassword.Token, resetPassword.Password!);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new { Errors = errors });
            }
            return Ok();
        }
        /// <summary>
        /// Confirms the user's email address.
        /// </summary>
        /// <param name="confirmEmailDTO">Email confirmation details.</param>
        /// <returns>Status message.</returns>
        [HttpGet("confirmEmail")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] ConfirmEmailDTO confirmEmailDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(confirmEmailDTO.Email);
            if (user == null)
                return BadRequest("Invalid Request");

            var result = await _userManager.ConfirmEmailAsync(user, confirmEmailDTO.Code);

            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(x => x.Description);
                return BadRequest(new { Errors = errors });
            }
            return Ok();
        }

        /// <summary>
        /// Adds a new role to the user.
        /// </summary>
        /// <param name="model">Role details to assign.</param>
        /// <returns>Status message.</returns>
        [HttpPost("addrole")]
        public async Task<IActionResult> AddRoleAsync([FromBody] AddRoleDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _authenticationServices.AddRoleToUserAsync(model);

            if (!string.IsNullOrEmpty(result))
                return BadRequest(result);

            return Ok(model);
        }
        /// <summary>
        /// Adds or updates the user's location.
        /// </summary>
        /// <param name="locationDTO">Location details.</param>
        /// <returns>Status message.</returns>
        [Authorize]
        [HttpPost("addLocation")]
        public async Task<IActionResult> AddLocation([FromBody] LocationDTO locationDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authenticationServices.AddLocationToUser(locationDTO);

            return Ok();
        }
        /// <summary>
        /// Removes the user's account.
        /// </summary>
        /// <returns>Status message.</returns>
        [Authorize]
        [HttpDelete("removeAccount")]
        public async Task<IActionResult> RemoveAccount()
        {
            await _authenticationServices.RemoveAccount();
            return Ok();
        }
        /// <summary>
        /// Refreshes the user's authentication token.
        /// </summary>
        /// <returns>New authentication token.</returns>
        [HttpGet("refreshtoken")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            if (string.IsNullOrEmpty(refreshToken))
                return BadRequest("Refresh token is missing or invalid.");

            var result = await _authenticationServices.RefreshTokenAsync(refreshToken);
            if (!result.IsAuthenticated)
                return BadRequest(result.Message);

            SetRefreshToken(result.RefreshToken, result.RefreshTokenExpiration);
            return Ok(result);
        }
        /// <summary>
        /// Revokes a refresh token.
        /// </summary>
        /// <param name="revokTokenDTO">Token details to revoke.</param>
        /// <returns>Status message.</returns>
        [HttpPost("revokeToken")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokTokenDTO revokTokenDTO)
        {
            var token = revokTokenDTO.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest("Token is required!");

            var result = await _authenticationServices.RevokeTokenAsync(token);

            if (!result)
                return BadRequest("Invalid token");

            return Ok();
        }
        /// <summary>
        /// Sets the refresh token cookie in the response.
        /// </summary>
        /// <param name="refreshToken">The refresh token string.</param>
        /// <param name="expires">The expiration time for the token.</param>
        private void SetRefreshToken(string refreshToken, DateTime expires)
        {
            var cookieOption = new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                Expires = expires
            };
            Response.Cookies.Append("refreshToken", refreshToken, cookieOption);
        }
    }
}
