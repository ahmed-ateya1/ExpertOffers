using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.CompanyDto;
using ExpertOffers.Core.DTOS.ClientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class CompanyConfig : Profile
    {
        public CompanyConfig()
        {
            CreateMap<CompanyUpdateRequest, Company>()
               .ForPath(dest => dest.User.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));


            CreateMap<Company, CompanyResponse>()
                .ForMember(dest=>dest.NumberOfBulletins,opt=>opt.MapFrom(src => src.Bulletins.Count))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.CompanyName))
                .ForPath(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForPath(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForPath(dest => dest.CountryName, opt => opt.MapFrom(src => src.User.Country.CountryName))
                .ForPath(dest => dest.CityName, opt => opt.MapFrom(src => src.User.City.CityName))
                .ForPath(dest => dest.IndustrialName, opt => opt.MapFrom(src => src.Industrial.IndustrialName));
        }
    }
}
