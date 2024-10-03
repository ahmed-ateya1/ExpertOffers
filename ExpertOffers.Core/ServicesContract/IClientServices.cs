using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.ClientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface IClientServices
    {
        Task<ClientReponse> UpdateAsync(ClientUpdateRequest? clientUpdateRequest);
        Task<bool> DeleteAsync();
        Task<ClientReponse> GetByAsync(Expression<Func<Client,bool>> expression , bool isTracking = true);

    }
}
