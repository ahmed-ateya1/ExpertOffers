using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.SavedItemDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection.Metadata;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using static Azure.Core.HttpHeader;

namespace ExpertOffers.Core.Services
{
    public class SavedItemServices : ISavedItemServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<SavedItemServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SavedItemServices
        (
            IUnitOfWork unitOfWork, 
            ILogger<SavedItemServices> logger, 
            IHttpContextAccessor httpContextAccessor
        )
        {
            _unitOfWork = unitOfWork;
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
                ?? throw new UnauthorizedAccessException("Client is not authenticated.");

            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(u => u.Email == email)
                ?? throw new UnauthorizedAccessException("Client is not authenticated.");

            var client = await _unitOfWork.Repository<Client>()
                .GetByAsync(c => c.UserID == user.Id)
                ?? throw new UnauthorizedAccessException("Client is not authenticated.");

            return client;
        }
       
        public async Task<SavedItemResponse> CreateAsync(SavedItemAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            ValidationHelper.ValidateModel(request);

            var client = await GetCurrentClientAsync();

            var offer = await _unitOfWork.Repository<Offer>()
                .GetByAsync(o => o.OfferID == request.ItemID, includeProperties: "Company");

            var savedItem = new SavedItem()
            {
                Client = client,
                SavedItemID = Guid.NewGuid(),
                ClientID = client.ClientID,
                SavedAt = DateTime.UtcNow,
            };

            var result = new SavedItemResponse()
            {
                ClientID = savedItem.ClientID,
                SavedAt = savedItem.SavedAt,
                SavedItemID = savedItem.SavedItemID,
            };
            if (offer != null)
            {
                var date = (int)(offer.EndDate - DateTime.UtcNow).TotalDays;
                savedItem.Offer = offer;
                savedItem.OfferId = offer.OfferID;
                result.ItemType = ItemOptions.OFFERS.ToString();
                result.ItemID = offer.OfferID;
                result.ItemName = offer.OfferTitle;
                result.ItemImageURL = offer.OfferPictureURL;
                result.ItemPrice = offer.OfferPrice;
                result.ItemDiscount = offer.OfferDiscount;
                result.TotalDayRemaining = date>0 ? date:0;
                result.CompanyID = offer.CompanyID;
                result.CompanyName = offer.Company.CompanyName;
                result.CompanyImageURL = offer.Company.CompanyLogoURL;
                savedItem.ItemType = ItemOptions.OFFERS;
                offer.TotalSaved++;

            }
            else
            {
                var coupon = await _unitOfWork.Repository<Coupon>()
                    .GetByAsync(c => c.CouponID == request.ItemID, includeProperties: "Company");

                if (coupon == null)
                {
                    throw new KeyNotFoundException("Item not found.");
                }
                var date = (int)(coupon.EndDate - DateTime.UtcNow).TotalDays;
                savedItem.Coupon = coupon;
                savedItem.CouponId = coupon.CouponID;

                result.ItemType = ItemOptions.COUPONS.ToString();
                result.ItemID = coupon.CouponID;
                result.ItemName = coupon.CouponTitle;
                result.ItemImageURL = coupon.CouponTitle;
                result.ItemImageURL = coupon.CouponePictureURL;
                result.ItemDiscount = coupon.DiscountPercentage;
                result.TotalDayRemaining = date>0?date:0;
                result.CompanyID = coupon.CompanyID;
                result.CompanyName = coupon.Company.CompanyName;
                result.CompanyImageURL = coupon.Company.CompanyLogoURL;
                savedItem.ItemType = ItemOptions.COUPONS;

                coupon.TotalSaved++;

            }
            await ExecuteWithTransaction(async () =>
            {
                await _unitOfWork.Repository<SavedItem>().CreateAsync(savedItem);
                await _unitOfWork.CompleteAsync();
            });

            return result;
        }

        public async Task<bool> DeleteAsync(Guid itemID)
        { 
            var offer = await _unitOfWork.Repository<Offer>()
                .GetByAsync(s => s.OfferID == itemID);
            SavedItem item = null;
            if (offer != null)
            {
                offer.TotalSaved--;
                item = await _unitOfWork.Repository<SavedItem>()
                    .GetByAsync(s => s.OfferId == itemID);
            }
            else
            {
                var coupon = await _unitOfWork.Repository<Coupon>()
                    .GetByAsync(s => s.CouponID == itemID);
                coupon.TotalSaved--;
                item = await _unitOfWork.Repository<SavedItem>()
                    .GetByAsync(s => s.CouponId == itemID);
            }
            var result = false;
            await ExecuteWithTransaction(async () =>
            {
                
                result = await _unitOfWork.Repository<SavedItem>().DeleteAsync(item);
                await _unitOfWork.CompleteAsync();
            });
            return result;
        }
    }
}
