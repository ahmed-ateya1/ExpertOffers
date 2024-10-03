using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.CityDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class CityConfig : Profile
    {
        public CityConfig()
        {
            CreateMap<CityAddRequest, City>()
                .ReverseMap();

            CreateMap<CityUpdateRequest, City>()
                 .ReverseMap();

            CreateMap<City , CityResponse>()
                .ForMember(dest=>dest.CountryName , opt=>opt.MapFrom(src=>src.Country.CountryName))
                .ForMember(dest => dest.CountryID, opt => opt.MapFrom(src => src.CountryID))
                .ReverseMap();
        }
    }
}
