using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IGenreOfferServices
    {
        Task<GenreResponse> CreateAsync(GenreAddRequest? genreRequest);
        Task<GenreResponse> UpdateAsync(GenreUpdateRequest? genreRequest);
        Task<bool> DeleteAsync(Guid? id);
        Task<IEnumerable<GenreResponse>> GetAllAsync(Expression<Func<GenreOffer,bool>>? expression=null);
        Task<GenreResponse> GetByAsync(Expression<Func<GenreOffer, bool>> expression , bool isTracked = true);
    }
}
