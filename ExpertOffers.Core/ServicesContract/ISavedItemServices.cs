using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.SavedItemDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface ISavedItemServices
    {
        Task<SavedItemResponse> CreateAsync(SavedItemAddRequest? request);
        Task<bool> DeleteAsync(Guid itemID);
    }
}
