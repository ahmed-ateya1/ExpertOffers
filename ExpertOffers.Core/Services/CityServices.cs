using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.CityDto;
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
    public class CityServices : ICityServices
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CityServices(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CityResponse> AddCityAsync(CityAddRequest? cityAddRequest)
        {
           if (cityAddRequest == null)
                throw new ArgumentNullException(nameof(cityAddRequest));

            ValidationHelper.ValidateModel(cityAddRequest);

            var countryFound = await _unitOfWork.Repository<Country>().GetByAsync(x=>x.CountryID == cityAddRequest.CountryID);
            if (countryFound == null)
                throw new ArgumentException(nameof(countryFound));

            var city =  _mapper.Map<City>(cityAddRequest);

            city.CityID = Guid.NewGuid();
            city.CountryID = cityAddRequest.CountryID;
            city.Country = countryFound;

            await _unitOfWork.Repository<City>().CreateAsync(city);

            return _mapper.Map<CityResponse>(city);
        }

        public async Task<bool> DeleteCityAsync(Guid? cityId)
        {
            if (cityId == null) throw new ArgumentNullException();

            var city = await _unitOfWork.Repository<City>().GetByAsync(x=>x.CityID == cityId);
            if(city == null)
                throw new ArgumentException(nameof(city));

            return await _unitOfWork.Repository<City>().DeleteAsync(city);
        }

        public async Task<IEnumerable<CityResponse>> GetAllAsync(Expression<Func<City, bool>>? predicate = null)
        {
            var cities = await _unitOfWork.Repository<City>().GetAllAsync(predicate , includeProperties: "Country");
            cities = cities.OrderBy(x=>x.CityName);
            return _mapper.Map<IEnumerable<CityResponse>>(cities);
        }

        public async Task<CityResponse> GetCityByAsync(Expression<Func<City, bool>> predicate, bool isTracked = true)
        {
            var city = await _unitOfWork.Repository<City>().GetByAsync(predicate , isTracked,includeProperties: "Country");
            return _mapper.Map<CityResponse>(city);
        }

        public async Task<CityResponse> UpdateCityAsync(CityUpdateRequest? cityUpdateRequest)
        {
            if(cityUpdateRequest == null)
                throw new ArgumentNullException();

            ValidationHelper.ValidateModel(cityUpdateRequest);

            var countryFound = await _unitOfWork.Repository<Country>().GetByAsync(x => x.CountryID == cityUpdateRequest.CountryID);
            if (countryFound == null)
                throw new ArgumentException(nameof(countryFound));

            var city = await _unitOfWork.Repository<City>().GetByAsync(x => x.CityID == cityUpdateRequest.CityID);
            if (city == null)
                throw new ArgumentException(nameof(city));

            city.CityName = cityUpdateRequest.CityName;
            city.CountryID = cityUpdateRequest.CountryID;

            await _unitOfWork.Repository<City>().UpdateAsync(city);
            return _mapper.Map<CityResponse>(city);
        }
    }
}
