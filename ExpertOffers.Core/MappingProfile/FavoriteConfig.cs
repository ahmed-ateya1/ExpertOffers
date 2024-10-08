using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.FavoriteDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class FavoriteConfig : Profile
    {
        public FavoriteConfig()
        {
            CreateMap<FavoriteAddRequest, Favorite>()
               .ForMember(dest => dest.FavoriteID, opt => opt.MapFrom(src => Guid.NewGuid()))
               .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => src.CompanyID))
               .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => DateTime.UtcNow))
               .ReverseMap();


            CreateMap<FavoriteUpdateRequest, Favorite>()
                .ForMember(dest => dest.FavoriteID, opt => opt.MapFrom(src =>src.FavoriteID))
                .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => src.CompanyID))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ReverseMap();

            CreateMap<Favorite, FavoriteResponse>()
                .ForMember(dest => dest.FavoriteID, opt => opt.MapFrom(src => src.FavoriteID))
                .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => src.CompanyID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.CompanyLogo, opt => opt.MapFrom(src => src.Company.CompanyLogoURL))
                .ForMember(dest => dest.DateAdded, opt => opt.MapFrom(src => src.DateAdded))
                .ForMember(dest=>dest.TotalBulletinForCompany , opt=>opt.MapFrom(src=>src.Company.Bulletins.Count))
                .ReverseMap();
        }
    }
}
