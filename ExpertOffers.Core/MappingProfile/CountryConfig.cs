using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.CountryDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class CountryConfig : Profile
    {
        public CountryConfig()
        {
            CreateMap<CountryAddRequest, Country>()
                .ReverseMap();

            CreateMap<Country , CountryResponse>()
                .ReverseMap();

            CreateMap<CountryUpdateRequest, Country>() .ReverseMap();
        }
    }
}
