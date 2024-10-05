using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.OfferDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IOfferServices
    {
        Task<OfferResponse> CreateAsync(OfferAddRequest? offerAddRequest);
        Task<OfferResponse> UpdateAsync(OfferUpdateRequest? offerUpdateRequest);
        Task<bool> DeleteAsync(Guid? offerID);
        Task<IEnumerable<OfferResponse>> GetAllAsync(Expression<Func<Offer,bool>>?expression = null);
        Task<OfferResponse> GetByAsync(Expression<Func<Offer, bool>> expression , bool isTracked=true);
    }
}
