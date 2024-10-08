using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BulletinGenreDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IBulletinGenreServices
    {
        Task<BulletinGenreResponse> CreateAsync(BulletinGenreAddRequest? request);
        Task<BulletinGenreResponse> UpdateAsync(BulletinGenreUpdateRequest? request);
        Task<bool> DeleteAsync(Guid? genreID);
        Task<IEnumerable<BulletinGenreResponse>> GetAllAsync(Expression<Func<BulletinGenre,bool>>?expression = null);
        Task<BulletinGenreResponse> GetByAsync(Expression<Func<BulletinGenre, bool>> expression , bool isTracked=false);
    }
}
