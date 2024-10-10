using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Domain.IdentityEntities;
using ExpertOffers.Core.Dtos.CompanyDto;
using ExpertOffers.Core.Dtos.CouponDto;
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
    public class CouponServices : ICouponServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFileServices _fileServices;
        private readonly ILogger<CouponServices> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CouponServices
            (
            IUnitOfWork unitOfWork,
            IMapper mapper,
            IFileServices fileServices,
            ILogger<CouponServices> logger,
            IHttpContextAccessor httpContextAccessor
            )

        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _fileServices = fileServices;
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
        private async Task<Company> GetCurrentCompanyAsync()
        {
            var email = _httpContextAccessor.HttpContext
                .User.FindFirstValue(ClaimTypes.Email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var user = await _unitOfWork.Repository<ApplicationUser>()
                .GetByAsync(u => u.Email == email)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            var company = await _unitOfWork.Repository<Company>()
                .GetByAsync(c => c.UserID == user.Id)
                ?? throw new UnauthorizedAccessException("User is not authenticated.");

            return company;
        }
        private async Task<GenreCoupon> CheckGenreIsValid(Guid genreID)
        {
            var genre = await _unitOfWork.Repository<GenreCoupon>()
                .GetByAsync(g => g.GenreID == genreID);
            return genre;
        }
        public async Task<CouponResponse> CreateAsync(CouponAddRequest? couponAddRequest)
        {
            if (couponAddRequest == null)
            {
                throw new ArgumentNullException(nameof(couponAddRequest));
            }
            ValidationHelper.ValidateModel(couponAddRequest);

            var company = await GetCurrentCompanyAsync();
            var genreCoupon = await CheckGenreIsValid(couponAddRequest.GenreID)
                ?? throw new ArgumentNullException(nameof(couponAddRequest.GenreID));

            var coupon = _mapper.Map<Coupon>(couponAddRequest);
            coupon.GenreCoupon = genreCoupon;
            coupon.Company = company;

            await ExecuteWithTransaction(async () =>
            {
                if(couponAddRequest.CouponePicture.Length > 0)
                {
                   coupon.CouponePictureURL =  await _fileServices.CreateFile(couponAddRequest.CouponePicture);
                }
                await _unitOfWork.Repository<Coupon>().CreateAsync(coupon);
            });
            var response = _mapper.Map<CouponResponse>(coupon);
            coupon.IsActive = response.IsActive;
            await _unitOfWork.CompleteAsync();
            return response;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var coupon = await _unitOfWork.Repository<Coupon>().GetByAsync(c => c.CouponID == id);
            if (coupon == null)
            {
                throw new ArgumentNullException(nameof(coupon));
            }
            var result = false;
            await ExecuteWithTransaction(async () =>
            {
                if(coupon.SavedItems.Count > 0)
                {
                    await _unitOfWork.Repository<SavedItem>().RemoveRangeAsync(coupon.SavedItems);
                }
                if(coupon.Notifications.Count > 0)
                {
                    await _unitOfWork.Repository<Notification>().RemoveRangeAsync(coupon.Notifications);
                }

                await _fileServices.DeleteFile(Path.GetFileName(coupon.CouponePictureURL));
                result = await _unitOfWork.Repository<Coupon>().DeleteAsync(coupon);
                await _unitOfWork.CompleteAsync();
            });
            return result;
        }

        public async Task<IEnumerable<CouponResponse>> GetAllAsync(Expression<Func<Coupon, bool>>? expression = null)
        {
            var coupons = await _unitOfWork.Repository<Coupon>().GetAllAsync(expression,includeProperties: "Company,GenreCoupon");
            coupons = coupons.OrderByDescending(x => x.DiscountPercentage);
            return _mapper.Map<IEnumerable<CouponResponse>>(coupons);
        }

        public async Task<CouponResponse> GetByAsync(Expression<Func<Coupon, bool>> expression, bool isTracked = true)
        {
            var coupon = await _unitOfWork.Repository<Coupon>().GetByAsync(expression, includeProperties: "Company,GenreCoupon");
            if (coupon != null)
            {
                coupon.TotalViews++;
                await _unitOfWork.CompleteAsync();
            }
            
            return _mapper.Map<CouponResponse>(coupon);
        }

        public async Task<CouponResponse> UpdateAsync(CouponUpdateRequest? couponUpdateRequest)
        {
            if (couponUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(couponUpdateRequest));
            }
            ValidationHelper.ValidateModel(couponUpdateRequest);
            var company = await GetCurrentCompanyAsync();
            var genreCoupon = await CheckGenreIsValid(couponUpdateRequest.GenreID)
                ?? throw new ArgumentNullException(nameof(couponUpdateRequest.GenreID));

            var coupon = await _unitOfWork.Repository<Coupon>().GetByAsync(c => c.CouponID == couponUpdateRequest.CouponID);
            if (coupon == null)
            {
                throw new ArgumentNullException(nameof(coupon));
            }

            _mapper.Map(couponUpdateRequest, coupon);
            coupon.GenreCoupon = genreCoupon;
            coupon.Company = company;
            await ExecuteWithTransaction(async () =>
            {
                if (couponUpdateRequest.CouponePicture != null
                && couponUpdateRequest.CouponePicture.Length > 0)
                {
                    coupon.CouponePictureURL = await _fileServices.UpdateFile(couponUpdateRequest.CouponePicture, Path.GetFileName(coupon.CouponePictureURL));
                }
                await _unitOfWork.Repository<Coupon>().UpdateAsync(coupon);
                await _unitOfWork.CompleteAsync();
            });
            var result =  _mapper.Map<CouponResponse>(coupon);
            coupon.IsActive = result.IsActive;
            await _unitOfWork.CompleteAsync();
            return result;
        }
    }
}
