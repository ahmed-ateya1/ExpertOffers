using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.CouponDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface ICouponServices
    {
        Task<CouponResponse> CreateAsync(CouponAddRequest? couponAddRequest);
        Task<CouponResponse> UpdateAsync(CouponUpdateRequest? couponUpdateRequest);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<CouponResponse>> GetAllAsync(Expression<Func<Coupon, bool>>? expression = null);
        Task<CouponResponse> GetByAsync(Expression<Func<Coupon, bool>> expression , bool isTracked = true);
    }
}
