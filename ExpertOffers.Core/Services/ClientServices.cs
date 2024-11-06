using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.DTOS.ClientDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class ClientServices : IClientServices
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<ClientServices> _logger;

        public ClientServices(IMapper mapper, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor, UserManager<ApplicationUser> userManager, ILogger<ClientServices> logger)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _logger = logger;
        }

        private async Task ExecuteWithTransaction(Func<Task> action)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await action();
                    await _unitOfWork.CommitTransactionAsync();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, ex.Message);
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
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

        private async Task<Client> GetClientByUserIdAsync(Guid userId)
        {
            var client = await _unitOfWork.Repository<Client>().GetByAsync(x => x.UserID == userId);
            if (client == null)
                throw new UnauthorizedAccessException("Client not found");
            return client;
        }

        private async Task<City> GetCityByIdAsync(Guid cityId)
        {
            var city = await _unitOfWork.Repository<City>().GetByAsync(x => x.CityID == cityId);
            if (city == null)
                throw new UnauthorizedAccessException("City not found");
            return city;
        }

        private async Task<Country> GetCountryByIdAsync(Guid countryId)
        {
            var country = await _unitOfWork.Repository<Country>().GetByAsync(x => x.CountryID == countryId);
            if (country == null)
                throw new UnauthorizedAccessException("Country not found");
            return country;
        }

        public async Task<bool> DeleteAsync(Guid clientID)
        {
            var client = await _unitOfWork.Repository<Client>()
                .GetByAsync(x => x.ClientID == clientID,includeProperties: "Favorites,Notifications,SavedItems,User");
            if (client == null)
                throw new UnauthorizedAccessException("Client not found");

            await ExecuteWithTransaction(async () =>
            {
                if (client.Favorites.Any())
                {
                    await _unitOfWork.Repository<Favorite>().RemoveRangeAsync(client.Favorites);
                }
                if (client.Notifications.Any())
                {
                    await _unitOfWork.Repository<Notification>().RemoveRangeAsync(client.Notifications);
                }
                if (client.SavedItems.Any())
                {
                    await _unitOfWork.Repository<SavedItem>().RemoveRangeAsync(client.SavedItems);
                }
                await _unitOfWork.Repository<Client>().DeleteAsync(client);
            });

            await _unitOfWork.Repository<ApplicationUser>().DeleteAsync(client.User);

            return true;
        }

        public async Task<ClientReponse> GetByAsync(Expression<Func<Client, bool>> expression, bool isTracking = true)
        {
            var client = await _unitOfWork.Repository<Client>().GetByAsync(expression);
            if (client == null)
                throw new UnauthorizedAccessException("Client not found");

            var user = await _unitOfWork.Repository<ApplicationUser>().GetByAsync(x => x.ClientID == client.ClientID , includeProperties: "Country,City");
            if (user == null)
                throw new UnauthorizedAccessException("User not found");

            client.User = user;

            return _mapper.Map<ClientReponse>(client);
        }

        public async Task<ClientReponse> UpdateAsync(ClientUpdateRequest clientUpdateRequest)
        {
            if (clientUpdateRequest == null)
                throw new ArgumentNullException(nameof(clientUpdateRequest));

            ValidationHelper.ValidateModel(clientUpdateRequest);

            var city = await GetCityByIdAsync(clientUpdateRequest.CityID);
            var country = await GetCountryByIdAsync(clientUpdateRequest.CountryID);

            var user = await GetUserAsync();
            var client = await GetClientByUserIdAsync(user.Id);

            await ExecuteWithTransaction(async () =>
            {
                user.CityID = clientUpdateRequest.CityID;
                user.City = city;
                user.Country = country;
                user.CountryID = clientUpdateRequest.CountryID;
                client.User = user;

                if (!string.IsNullOrEmpty(clientUpdateRequest.PhoneNumber))
                    user.PhoneNumber = clientUpdateRequest.PhoneNumber;

                _mapper.Map(clientUpdateRequest, client);

                await _unitOfWork.Repository<Client>().UpdateAsync(client);
                await _unitOfWork.CompleteAsync();
            });

            return _mapper.Map<ClientReponse>(client);
        }
    }
}
