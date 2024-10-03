using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.CountryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.ServicesContract
{
    public interface ICountryServices
    {
        Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest);
        Task<CountryResponse> UpdateCountry(CountryUpdateRequest? countryUpdateRequest);
        Task<bool> DeleteCountry(Guid countryID);
        Task<CountryResponse> GetCountry(Expression<Func<Country , bool>> expression , bool isTracked = true);
        Task<IEnumerable<CountryResponse>> GetCountries(Expression<Func<Country, bool>>? expression = null);
    }
}
