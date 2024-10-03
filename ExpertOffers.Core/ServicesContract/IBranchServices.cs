using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BranchDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IBranchServices 
    {
        Task<BranchResponse> CreateAsync(BranchAddRequest? branchAddRequest);
        Task<BranchResponse> UpdateAsync(BranchUpdateRequest? branchUpdateRequest);
        Task<BranchResponse> GetByAsync(Expression<Func<Branch, bool>>expression , bool isTracked = true);
        Task<List<BranchResponse>> GetAllAsync(Expression<Func<Branch , bool>>? expression=null);
        Task<bool> DeleteBranchAsync(Guid branchID);
    }
}
