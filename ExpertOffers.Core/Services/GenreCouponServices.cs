using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreCouponDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class GenreCouponServices : IGenreCouponServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<GenreCouponServices> _logger;

        public GenreCouponServices(
            IUnitOfWork unitOfWork,
            IMapper mapper, 
            ILogger<GenreCouponServices> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
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

        public async Task<GenreCouponResponse> CreateAsync(GenreCouponAddRequest? request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            ValidationHelper.ValidateModel(request);

            var genreCoupon = _mapper.Map<GenreCoupon>(request);
            await ExecuteWithTransaction(async () =>
            {
                await _unitOfWork.Repository<GenreCoupon>().CreateAsync(genreCoupon);
                await _unitOfWork.CompleteAsync();
            });

            return _mapper.Map<GenreCouponResponse>(genreCoupon);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var genreCoupon = await _unitOfWork.Repository<GenreCoupon>()
                .GetByAsync(x=>x.GenreID == id);
            if(genreCoupon == null)
            {
                throw new ArgumentNullException(nameof(genreCoupon));
            }
            var result = false;
            await ExecuteWithTransaction(async () =>
            {
                if (genreCoupon.Coupons.Any())
                {
                    await _unitOfWork.Repository<Coupon>().RemoveRangeAsync(genreCoupon.Coupons);
                }
                result = await _unitOfWork.Repository<GenreCoupon>().DeleteAsync(genreCoupon);
                await _unitOfWork.CompleteAsync();
            });
            return result;

        }

        public async Task<IEnumerable<GenreCouponResponse>> GetAllAsync(Expression<Func<GenreCoupon, bool>>? expression = null)
        {
           var response = await _unitOfWork.Repository<GenreCoupon>().GetAllAsync(expression);
            return _mapper.Map<IEnumerable<GenreCouponResponse>>(response);
        }

        public async Task<GenreCouponResponse> GetByAsync(Expression<Func<GenreCoupon, bool>> expression, bool isTracked = true)
        {
           var response = await _unitOfWork.Repository<GenreCoupon>().GetByAsync(expression, isTracked);
            return _mapper.Map<GenreCouponResponse>(response);
        }

        public async Task<GenreCouponResponse> UpdateAsync(GenreCouponUpdateRequest? request)
        {
           if(request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            ValidationHelper.ValidateModel(request);

            var genreCoupon = _mapper.Map<GenreCoupon>(request);
            await ExecuteWithTransaction(async () =>
            {
                await _unitOfWork.Repository<GenreCoupon>().UpdateAsync(genreCoupon);
                await _unitOfWork.CompleteAsync();
            });
            return _mapper.Map<GenreCouponResponse>(genreCoupon);
        }
    }
}
