using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreCouponDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IGenreCouponServices
    {
        Task<GenreCouponResponse> CreateAsync(GenreCouponAddRequest? request);
        Task<GenreCouponResponse> UpdateAsync(GenreCouponUpdateRequest? request);
        Task<bool> DeleteAsync(Guid id);
        Task<GenreCouponResponse> GetByAsync(Expression<Func<GenreCoupon , bool>>expression , bool isTracked = true);
        Task<IEnumerable<GenreCouponResponse>> GetAllAsync(Expression<Func<GenreCoupon, bool>>? expression = null);

    }
}
