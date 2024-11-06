using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BulletinDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class BulletinConfig : Profile
    {
        public BulletinConfig()
        {
            CreateMap<BulletinAddRquest, Bulletin>()
                .ForMember(dest => dest.BulletinID, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.BulletinTitle, opt => opt.MapFrom(src => src.BulletinTitle))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ReverseMap();

            CreateMap<BulletinUpdateRequest, Bulletin>()
                .ForMember(dest => dest.BulletinID, opt => opt.MapFrom(src => src.BulletinID))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.BulletinTitle, opt => opt.MapFrom(src => src.BulletinTitle))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ReverseMap();

            CreateMap<Bulletin, BulletinResponse>()
                .ForMember(dest => dest.BulletinID, opt => opt.MapFrom(src => src.BulletinID))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.BulletinTitle, opt => opt.MapFrom(src => src.BulletinTitle))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.BulletinPictureUrl, opt => opt.MapFrom(src => src.BulletinPictureUrl))
                .ForMember(dest => dest.BulletinPdfUrl, opt => opt.MapFrom(src => src.BulletinPdfUrl))
                .ForMember(dest=>dest.GenreName, opt => opt.MapFrom(src => src.Genre.GenreName))
                .ForMember(dest=>dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest=>dest.CompanyLogoURL, opt => opt.MapFrom(src => src.Company.CompanyLogoURL))
                .ForMember(dest=>dest.CompanyID, opt => opt.MapFrom(src => src.CompanyID))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.CheckIsActive()))
                .ForMember(dest => dest.NumOfDaysRemaining, opt => opt.MapFrom(src => src.GetDaysRemaining()))
                .ReverseMap();


        }
    }
}
