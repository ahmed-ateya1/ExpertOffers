using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.DTOS.ClientDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class ClientServices : IClientServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;

        public ClientServices(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
        }

        private async Task<ApplicationUser> GetUserAsync()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrEmpty(email))
                throw new UnauthorizedAccessException("User is not authenticated");

            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(x => x.Email == email, includeProperties: "Country,City");
            if (user == null)
                throw new UnauthorizedAccessException("User is not authenticated");
            return user;
        }

        public async Task<bool> DeleteAsync()
        {
            var user = await GetUserAsync();
            var client = await _unitOfWork.Repository<Client>().GetByAsync(x => x.UserID == user.Id);
            if (client == null)
                throw new UnauthorizedAccessException("Client not found");

            return await _unitOfWork.Repository<Client>().DeleteAsync(client);
        }

        public async Task<ClientReponse> GetByAsync(Expression<Func<Client, bool>> expression, bool isTracking = true)
        {
            var user = await GetUserAsync();
            var client = await _unitOfWork.Repository<Client>().GetByAsync(expression);
            client.User = user;
            if (client == null)
                throw new UnauthorizedAccessException("Client not found");
            return _mapper.Map<ClientReponse>(client);
        }

        public async Task<ClientReponse> UpdateAsync(ClientUpdateRequest clientUpdateRequest)
        {
            if (clientUpdateRequest == null)
                throw new ArgumentNullException(nameof(clientUpdateRequest));

            ValidationHelper.ValidateModel(clientUpdateRequest);
            var city = await _unitOfWork.Repository<City>().GetByAsync(x => x.CityID == clientUpdateRequest.CityID);
            var country = await _unitOfWork.Repository<Country>().GetByAsync(x => x.CountryID == clientUpdateRequest.CountryID);
            if (city == null || country == null)
                throw new UnauthorizedAccessException("City or Country not found");

            var user = await GetUserAsync();
            var clientUpdate = await _unitOfWork.Repository<Client>().GetByAsync(x => x.UserID == user.Id);
            clientUpdate.User = user;
            user.CityID = clientUpdateRequest.CityID;
            user.City = city;
            user.Country = country;
            user.CountryID = clientUpdateRequest.CountryID;
            user.PhoneNumber = clientUpdateRequest.PhoneNumber;
            await _unitOfWork.CompleteAsync();

            _mapper.Map(clientUpdateRequest, clientUpdate);

            await _unitOfWork.Repository<Client>().UpdateAsync(clientUpdate);

            return _mapper.Map<ClientReponse>(clientUpdate);
        }
    }

}
