using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.DTOS.ClientDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class ClientConfig : Profile
    {
        public ClientConfig()
        {

            CreateMap<ClientUpdateRequest, Client>()
                .ForPath(dest => dest.User.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            
            CreateMap<Client, ClientReponse>()
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForPath(dest => dest.Email, opt => opt.MapFrom(src => src.User.Email))
                .ForPath(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.User.PhoneNumber))
                .ForPath(dest => dest.CountryName, opt => opt.MapFrom(src => src.User.Country.CountryName))
                .ForPath(dest => dest.CityName, opt => opt.MapFrom(src => src.User.City.CityName));
        }
    }


}
