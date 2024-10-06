using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.CouponDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class CouponConfig : Profile
    {
        public CouponConfig()
        {
            CreateMap<CouponAddRequest , Coupon>()
                .ForMember(dest => dest.CouponID, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.CouponTitle, opt => opt.MapFrom(src => src.CouponTitle))
                .ForMember(dest => dest.TotalSaved, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalViews, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.CouponCode))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.CouponeURL, opt => opt.MapFrom(src => src.CouponeURL))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ReverseMap();

            CreateMap<CouponUpdateRequest, Coupon>()
                .ForMember(dest => dest.CouponTitle, opt => opt.MapFrom(src => src.CouponTitle))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => false))
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.CouponCode))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.CouponeURL, opt => opt.MapFrom(src => src.CouponeURL))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ReverseMap();

            CreateMap<Coupon , CouponResponse>()
                .ForMember(dest => dest.CouponID, opt => opt.MapFrom(src => src.CouponID))
                .ForMember(dest => dest.CouponTitle, opt => opt.MapFrom(src => src.CouponTitle))
                .ForMember(dest => dest.CouponCode, opt => opt.MapFrom(src => src.CouponCode))
                .ForMember(dest => dest.DiscountPercentage, opt => opt.MapFrom(src => src.DiscountPercentage))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.NumOfDaysRemaining, opt => opt.MapFrom(src => src.EndDate > DateTime.Now ? (src.EndDate - DateTime.Now).Days : 0))
                .ForMember(dest => dest.CouponePictureURL, opt => opt.MapFrom(src => src.CouponePictureURL))
                .ForMember(dest => dest.CouponeURL, opt => opt.MapFrom(src => src.CouponeURL))
                .ForMember(dest => dest.TotalViews, opt => opt.MapFrom(src => src.TotalViews))
                .ForMember(dest => dest.TotalSaved, opt => opt.MapFrom(src => src.TotalSaved))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.StartDate <= DateTime.Now && src.EndDate >= DateTime.Now))
                .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src => src.CompanyID))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.CompayLogoURL, opt => opt.MapFrom(src => src.Company.CompanyLogoURL))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.GenreCoupon.GenreName))
                .ReverseMap();
        }
    }
}
