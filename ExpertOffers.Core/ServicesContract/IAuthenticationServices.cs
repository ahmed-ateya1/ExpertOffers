using ExpertOffers.Core.DTOS.AuthenticationDTO;
using ExpertOffers.Core.DTOS.CityDto;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IAuthenticationServices
    {
        Task<AuthenticationResponse> RegisterClientAsync(ClientRegisterDTO clientRegisterDTO);
        Task<AuthenticationResponse> RegisterCompanyAsync(CompanyRegisterDTO companyRegisterDTO);
        Task<AuthenticationResponse> LoginAsync(LoginDTO loginDTO);
        Task<AuthenticationResponse> RefreshTokenAsync(string token);
        Task<bool> ForgotPassword(ForgotPasswordDTO forgotPasswordDTO);
        Task RemoveAccount();
        Task AddLocationToUser(LocationDTO locationDTO);
        Task<bool> RevokeTokenAsync(string token);
        Task<string> AddRoleToUserAsync(AddRoleDTO model);
    }
}
