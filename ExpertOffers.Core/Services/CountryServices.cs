using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.CityDto;
using ExpertOffers.Core.DTOS.CountryDto;
using ExpertOffers.Core.Helper;
using ExpertOffers.Core.IUnitOfWorkConfig;
using ExpertOffers.Core.ServicesContract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.Services
{
    public class CountryServices : ICountryServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CountryServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CountryResponse> AddCountry(CountryAddRequest? countryAddRequest)
        {
            if (countryAddRequest == null)
            {
                throw new ArgumentNullException(nameof(countryAddRequest));
            }
            ValidationHelper.ValidateModel(countryAddRequest);

            var country = _mapper.Map<Country>(countryAddRequest);

            country.CountryID = Guid.NewGuid();

            await _unitOfWork.Repository<Country>().CreateAsync(country);

            return _mapper.Map<CountryResponse>(country);

        }

        public async Task<bool> DeleteCountry(Guid countryID)
        {
            var country = await _unitOfWork.Repository<Country>().GetByAsync(x=>x.CountryID == countryID);
            if(country == null)
            {
                throw new Exception("Country not found");
            }
            return await _unitOfWork.Repository<Country>().DeleteAsync(country);
        }

        public async Task<IEnumerable<CountryResponse>> GetCountries(Expression<Func<Country, bool>>? expression = null)
        {

            var countries =  await _unitOfWork.Repository<Country>().GetAllAsync(expression);
            countries = countries.OrderBy(x=>x.CountryName).ToList();
            return _mapper.Map<IEnumerable<CountryResponse>>(countries);
        }

        public async Task<CountryResponse> GetCountry(Expression<Func<Country, bool>> expression, bool isTracked = true)
        {
            var country = _unitOfWork.Repository<Country>().GetByAsync(expression, isTracked);
            return _mapper.Map<CountryResponse>(country);
        }

        public async Task<CountryResponse> UpdateCountry(CountryUpdateRequest? countryUpdateRequest)
        {
           if (countryUpdateRequest == null)
            {
                throw new ArgumentNullException(nameof(countryUpdateRequest));
            }
            ValidationHelper.ValidateModel(countryUpdateRequest);

            var country = await _unitOfWork.Repository<Country>().GetByAsync(x => x.CountryID == countryUpdateRequest.CountryID);
            if (country == null)
                throw new ArgumentException(nameof(country));

            country.CountryName = countryUpdateRequest.CountryName;

          await _unitOfWork.Repository<Country>().UpdateAsync(country);

            return _mapper.Map<CountryResponse>(country);
        }
    }
}
