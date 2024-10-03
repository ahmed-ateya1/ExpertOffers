using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.CityDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface ICityServices
    {
        Task<CityResponse> AddCityAsync(CityAddRequest? cityAddRequest);
        Task<CityResponse> UpdateCityAsync(CityUpdateRequest? cityUpdateRequest);
        Task<bool> DeleteCityAsync(Guid? cityId);
        Task<CityResponse> GetCityByAsync(Expression<Func<City , bool>> predicate , bool isTracked = true);
        Task<IEnumerable<CityResponse>> GetAllAsync(Expression<Func<City, bool>>? predicate = null);
    }
}
