using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.CompanyDto;
using ExpertOffers.Core.Dtos.FavoriteDto;
using ExpertOffers.Core.Dtos.OfferDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
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
    public class OfferServices : IOfferServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<OfferServices> _logger;
        private readonly IFileServices _fileServices;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public OfferServices(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<OfferServices> logger,
            IFileServices fileServices,
            IHttpContextAccessor httpContextAccessor)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
            _fileServices = fileServices;
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
        private async Task<Client?> GetCurrentClientAsync()
        {
            var email = _httpContextAccessor.HttpContext.User.FindFirstValue(ClaimTypes.Email);

            if (email == null)
                return null;

            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(x => x.Email == email, includeProperties: "Country,City");

            if (user == null)
                return null;

            var client = await _unitOfWork.Repository<Client>()
                .GetByAsync(x => x.ClientID == user.ClientID);

            return client;
        }

        private async Task SetSavedItemAsync(List<OfferResponse> offers, Guid clientID)
        {
            var offerIds = offers.Select(o => o.OfferID).ToList();
            var savedOffers = await _unitOfWork.Repository<SavedItem>()
                    .GetAllAsync(s => s.ClientID == clientID && offerIds.Contains(s.OfferId.Value));

            var ids = savedOffers.Select(s => s.OfferId).ToList();

            foreach (var offer in offers)
            {
                offer.CurrentUserIsSaved = ids.Contains(offer.OfferID);
            }
        }
        private async Task<Company> GetCurrentCompanyAsync()
        {
            var email = _httpContextAccessor.HttpContext
                .User.FindFirstValue(ClaimTypes.Email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");
            
            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(u => u.Email == email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(c => c.CompanyID == user.ComapnyID)??
                throw new UnauthorizedAccessException("User is not a company.");


            return user.Company;
        }
        private async Task<GenreOffer> CheckGenreIsValid(Guid genreID)
        {
            var genre = await _unitOfWork.Repository<GenreOffer>()
                .GetByAsync(g => g.GenreID == genreID);
            return genre;
        }
        private string GetBaseUrl()
        {
            var request = _httpContextAccessor.HttpContext.Request;
            return $"{request.Scheme}://{request.Host.Value}/api/";
        }
        private async Task HandleNotificationAsync(Offer offer)
        {
            var favorites = await _unitOfWork.Repository<Favorite>()
                .GetAllAsync(f => f.CompanyID == offer.CompanyID, includeProperties: "Client");

            var notifications = favorites.Select(f => new Notification
            {
                ClientID = f.Client.ClientID,
                OfferId = offer.OfferID,
                Message = $"New offer from {offer.Company.CompanyName}",
                CreatedDate = DateTime.Now,
                IsRead = false,
                NotificationID = Guid.NewGuid(),
                NotificationType = NotificationOptions.NEW_OFFER.ToString(),
                ReferenceURL = $"{GetBaseUrl()}Offer/getOffer/{offer.OfferID}"
            }).ToList();

            await _unitOfWork.Repository<Notification>().AddRangeAsync(notifications);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<OfferResponse> CreateAsync(OfferAddRequest? offerAddRequest)
        {
            if (offerAddRequest == null)
            {
                throw new ArgumentNullException(nameof(offerAddRequest));
            }
            ValidationHelper.ValidateModel(offerAddRequest);

            Company company = null;
            if (offerAddRequest.CompanyID==null)
            {
               company = await GetCurrentCompanyAsync();
            }
            else
            {
                company = await _unitOfWork.Repository<Company>()
                    .GetByAsync(c => c.CompanyID == offerAddRequest.CompanyID);
            }
            var genreOffer = await CheckGenreIsValid(offerAddRequest.GenreID)
                ?? throw new ArgumentNullException(nameof(offerAddRequest.GenreID));

            var offer = _mapper.Map<Offer>(offerAddRequest);

            await ExecuteWithTransaction(async () =>
            {
                if (offerAddRequest.OfferPicture.Length > 0)
                {
                    offer.OfferPictureURL = await _fileServices.CreateFile(offerAddRequest.OfferPicture);
                }
                offer.CompanyID = company.CompanyID;
                offer.Company = company;
                offer.GenreID = genreOffer.GenreID;
                offer.Genre = genreOffer;

                await _unitOfWork.Repository<Offer>().CreateAsync(offer);


                await _unitOfWork.CompleteAsync();

                await HandleNotificationAsync(offer);


            });

            var result = _mapper.Map<OfferResponse>(offer);
            offer.IsActive = result.IsActive;
            await _unitOfWork.CompleteAsync();

            return result;
        }


        public async Task<bool> DeleteAsync(Guid? offerID)
        {
            var offer = await _unitOfWork.Repository<Offer>()
                .GetByAsync(o => o.OfferID == offerID,includeProperties: "SavedItems,Notifications")
                ?? throw new ArgumentNullException(nameof(offerID));
            var result = false;
            await ExecuteWithTransaction(async () =>
            {
                string fileName = new Uri(offer.OfferPictureURL).Segments.Last();
                await _fileServices.DeleteFile(fileName);
                if(offer.SavedItems != null)
                {
                    await _unitOfWork.Repository<SavedItem>()
                    .RemoveRangeAsync(offer.SavedItems);    
                }
                if (offer.Notifications != null)
                {
                    await _unitOfWork.Repository<Notification>()
                    .RemoveRangeAsync(offer.Notifications);
                }
                result = await _unitOfWork.Repository<Offer>().DeleteAsync(offer);
                await _unitOfWork.CompleteAsync();
            });
            return result;
        }

        public async Task<IEnumerable<OfferResponse>> GetAllAsync(Expression<Func<Offer, bool>>? expression = null)
        {
            var result = await _unitOfWork.Repository<Offer>().GetAllAsync(expression,includeProperties: "Company,Genre,Company.User,Company.User.Country");
            result = result.OrderByDescending(x=>x.OfferDiscount);

            var res = _mapper.Map<IEnumerable<OfferResponse>>(result);
            var client = await GetCurrentClientAsync();
            if (client != null)
            {
                await SetSavedItemAsync(res.ToList(), client.ClientID);
            }
            return res;
        }

        public async Task<OfferResponse> GetByAsync(Expression<Func<Offer, bool>> expression, bool isTracked = true)
        {
            var result =  await _unitOfWork.Repository<Offer>().GetByAsync(expression, isTracked, includeProperties: "Company,Genre");
            result.TotalViews++;
            await _unitOfWork.CompleteAsync();

            var res = _mapper.Map<OfferResponse>(result);
            var client = await GetCurrentClientAsync();
            if (client != null)
            {
                var savedItem = await _unitOfWork.Repository<SavedItem>()
                    .GetByAsync(s => s.ClientID == client.ClientID && s.OfferId == result.OfferID);
                res.CurrentUserIsSaved = savedItem != null;
            }
            return res;
        }

        public async Task<OfferResponse> UpdateAsync(OfferUpdateRequest? offerUpdateRequest)
        {
            if (offerUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(offerUpdateRequest));
            }
            ValidationHelper.ValidateModel(offerUpdateRequest);

            var offer = await _unitOfWork.Repository<Offer>()
                .GetByAsync(x => x.OfferID == offerUpdateRequest.OfferID) 
                ?? throw new ArgumentNullException(nameof(offerUpdateRequest.OfferID));


            Company company = null;
            if (offerUpdateRequest.CompanyID == null)
            {
                company = await GetCurrentCompanyAsync();
            }
            else
            {
                company = await _unitOfWork.Repository<Company>()
                    .GetByAsync(c => c.CompanyID == offerUpdateRequest.CompanyID);
            }
            var genreOffer = await CheckGenreIsValid(offerUpdateRequest.GenreID)
                ?? throw new ArgumentNullException(nameof(offerUpdateRequest.GenreID));

            _mapper.Map(offerUpdateRequest, offer);
            await ExecuteWithTransaction(async () =>
            {
                if (offerUpdateRequest.OfferPicture!=null
                &&offerUpdateRequest.OfferPicture.Length > 0)
                {
                    offer.OfferPictureURL = await _fileServices.UpdateFile(offerUpdateRequest.OfferPicture,Path.GetFileName(offer.OfferPictureURL));
                }
                offer.CompanyID = company.CompanyID;
                offer.Company = company;
                offer.GenreID = genreOffer.GenreID;
                offer.Genre = genreOffer;
                await _unitOfWork.Repository<Offer>().UpdateAsync(offer);
                await _unitOfWork.CompleteAsync();
            });

            var result =  _mapper.Map<OfferResponse>(offer);
            offer.IsActive = result.IsActive;
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}
