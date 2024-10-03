using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.CompanyDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface ICompanyServices
    {
        Task<CompanyResponse> UpdateAsync(CompanyUpdateRequest? request);
        Task<CompanyResponse> GetByAsync(Expression<Func<Company, bool>> expression, bool isTracked = true);
        Task<IEnumerable<CompanyResponse>> GetAllAsync(Expression<Func<Company, bool>>? expression = null);

    }
}
