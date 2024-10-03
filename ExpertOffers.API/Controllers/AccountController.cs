using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.DTOS.AuthenticationDTO;
using ExpertOffers.Core.DTOS.CityDto;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Facebook;
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
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAuthenticationServices _authenticationServices;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly SignInManager<ApplicationUser> _signInManager;

       
        public AccountController(IAuthenticationServices authenticationServices, UserManager<ApplicationUser> userManager, IEmailSender emailSender , SignInManager<ApplicationUser> signInManager )
        {
            _authenticationServices = authenticationServices;
            _userManager = userManager;
            _emailSender = emailSender;
            _signInManager = signInManager;
        }

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
        [HttpPost("addLocation")]
        public async Task<IActionResult> AddLocation([FromBody] LocationDTO locationDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _authenticationServices.AddLocationToUser(locationDTO);

            return Ok();
        }
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
