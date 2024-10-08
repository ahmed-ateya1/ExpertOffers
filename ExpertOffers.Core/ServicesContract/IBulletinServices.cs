using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BulletinDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IBulletinServices
    {
        Task<BulletinResponse?> CreateAsync(BulletinAddRquest? request);
        Task<BulletinResponse> UpdateAsync(BulletinUpdateRequest? request);
        Task<bool> DeleteAsync(Guid? id);
        Task<IEnumerable<BulletinResponse>> GetAllAsync(Expression<Func<Bulletin,bool>>?expression=null);
        Task<BulletinResponse> GetByAsync(Expression<Func<Bulletin, bool>> expression,bool isTracked = false);
    }
}
