using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.FavoriteDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class FavoriteServices : IFavoriteServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<FavoriteServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public FavoriteServices(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<FavoriteServices> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }
        private async Task ExecuteWithTransaction(Func<Task> action)
        {
            using (var transaction = await _unitOfWork.BeginTransactionAsync())
            {
                try
                {
                    await action();
                    await _unitOfWork.CommitTransactionAsync();
                    _logger.LogInformation("Transaction committed successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Transaction failed: {ErrorMessage}", ex.Message);
                    await _unitOfWork.RollbackTransactionAsync();
                    throw;
                }
            }
        }
        private async Task<Client> GetCurrentClientAsync()
        {
            var email = _httpContextAccessor.HttpContext
                .User.FindFirstValue(ClaimTypes.Email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(u => u.Email == email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var client = await _unitOfWork.Repository<Client>()
                .GetByAsync(c => c.UserID == user.Id)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            return client;
        }
        private async Task<Company> CheckCompanyIsValidAsync(Guid companyID)
        {
            return await _unitOfWork.Repository<Company>()
                .GetByAsync(c => c.CompanyID == companyID ,includeProperties: "Bulletins")
                ?? throw new KeyNotFoundException("Company not found.");
        }
        
        public async Task<FavoriteResponse> AddFavorite(FavoriteAddRequest? favoriteAdd)
        {
            if(favoriteAdd == null)
            {
                throw new ArgumentNullException(nameof(favoriteAdd));
            }
            ValidationHelper.ValidateModel(favoriteAdd);
            
            var company = await CheckCompanyIsValidAsync(favoriteAdd.CompanyID);
            var client = await GetCurrentClientAsync();
            var existingFavorite = await _unitOfWork.Repository<Favorite>()
                    .GetByAsync(f => f.CompanyID == company.CompanyID 
                    && f.ClientID == client.ClientID);

            if (existingFavorite != null)
            {
                throw new InvalidOperationException("This company is already in your favorites.");
            }
            var favorite = _mapper.Map<Favorite>(favoriteAdd);
            favorite.Company = company;
            favorite.Client = client;
            favorite.CompanyID = company.CompanyID;
            favorite.ClientID = client.ClientID;
            
            await ExecuteWithTransaction(async () =>
            {
                await _unitOfWork.Repository<Favorite>().CreateAsync(favorite);
                await _unitOfWork.CompleteAsync();
            });

            var notification = new Notification()
            {
                CompanyID = company.CompanyID,
                Message = $"{client.FirstName + " " + client.LastName} has added your company to his favorites list.",
                CreatedDate = DateTime.Now,
                IsRead = false,
                NotificationType = NotificationOptions.NEW_FAV.ToString(),
                NotificationID = Guid.NewGuid()
            };
            await _unitOfWork.Repository<Notification>().CreateAsync(notification);
            await _unitOfWork.CompleteAsync();
            return _mapper.Map<FavoriteResponse>(favorite);
        }

        public async Task<IEnumerable<FavoriteResponse>> GetAllAsync(Expression<Func<Favorite, bool>>? expression = null)
        {
            var result = await _unitOfWork.Repository<Favorite>()
                .GetAllAsync(expression , includeProperties: "Company,Client,Company.Bulletins");

            return _mapper.Map<IEnumerable<FavoriteResponse>>(result);
        }

        public async Task<FavoriteResponse> GetByAsync(Expression<Func<Favorite, bool>>? expression = null, bool isTracked = false)
        {
            var result = await _unitOfWork.Repository<Favorite>()
                .GetByAsync(expression, isTracked, includeProperties: "Company,Client,Company.Bulletins");
            return _mapper.Map<FavoriteResponse>(result);
        }

        public async Task<bool> RemoveFavorite(Guid? id)
        {
            var client = await GetCurrentClientAsync();
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }
            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(x => x.CompanyID == id);
            if(company == null)
            {
                return false;
            }

            var favorite = await _unitOfWork.Repository<Favorite>()
                .GetByAsync(f => f.CompanyID == company.CompanyID && f.ClientID == client.ClientID);
            if (favorite == null)
            {
                return false;
            }
            bool result = false;
            await ExecuteWithTransaction(async () =>
            {
                result = await _unitOfWork.Repository<Favorite>().DeleteAsync(favorite);
                await _unitOfWork.CompleteAsync();
            });
            return result;
        }
    }
}
