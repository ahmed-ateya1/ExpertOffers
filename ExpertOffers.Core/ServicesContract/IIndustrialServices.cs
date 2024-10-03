using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.IndustrialDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IIndustrialServices
    {
        Task<IndustrialResponse> CreateAsync(IndustrialAddRequest? industrialAddRequest);
        Task<IndustrialResponse> UpdateAsync(IndustrialUpdateRequest? industrialUpdateRequest);
        Task<bool> DeleteAsync(Guid? id);
        Task<IndustrialResponse> GetByAsync(Expression<Func<Industrial,bool>> expression , bool isTracked = true);
        Task<List<IndustrialResponse>> GetAllAsync(Expression<Func<Industrial, bool>>? expression=null);
    }
}
