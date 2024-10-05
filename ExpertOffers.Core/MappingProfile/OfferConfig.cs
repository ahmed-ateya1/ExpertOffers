using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.OfferDto;
using System;

namespace ExpertOffers.Core.MappingProfile
{
    public class OfferConfig : Profile
    {
        public OfferConfig()
        {
            CreateMap<OfferAddRequest, Offer>()
                .ForMember(dest => dest.OfferID, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.OfferTitle, opt => opt.MapFrom(src => src.OfferTitle))
                .ForMember(dest => dest.OfferPrice, opt => opt.MapFrom(src => src.OfferPrice))
                .ForMember(dest => dest.OfferDiscount, opt => opt.MapFrom(src => src.OfferDiscount))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.TotalViews, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.TotalSaved, opt => opt.MapFrom(src => 0))
                .ReverseMap();

            CreateMap<OfferUpdateRequest , Offer>()
                .ForMember(dest => dest.OfferID, opt => opt.MapFrom(src => src.OfferID))
                .ForMember(dest => dest.OfferTitle, opt => opt.MapFrom(src => src.OfferTitle))
                .ForMember(dest => dest.OfferPrice, opt => opt.MapFrom(src => src.OfferPrice))
                .ForMember(dest => dest.OfferDiscount, opt => opt.MapFrom(src => src.OfferDiscount))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ReverseMap();
            CreateMap<Offer, OfferResponse>()
                .ForMember(dest => dest.OfferID, opt => opt.MapFrom(src => src.OfferID))
                .ForMember(dest => dest.OfferTitle, opt => opt.MapFrom(src => src.OfferTitle))
                .ForMember(dest => dest.OfferPrice, opt => opt.MapFrom(src => src.OfferPrice))
                .ForMember(dest => dest.OfferPricaBeforeDiscount, opt => opt.MapFrom(src => src.OfferPrice * (src.OfferDiscount / 100) + src.OfferPrice))
                .ForMember(dest => dest.OfferDiscount, opt => opt.MapFrom(src => src.OfferDiscount))
                .ForMember(dest => dest.OfferPictureURL, opt => opt.MapFrom(src => src.OfferPictureURL))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.NumOfDaysRemaining, opt => opt.MapFrom(src => src.EndDate > DateTime.Now ? (src.EndDate - DateTime.Now).Days : 0))
                .ForMember(dest => dest.TotalViews, opt => opt.MapFrom(src => src.TotalViews))
                .ForMember(dest => dest.TotalSaved, opt => opt.MapFrom(src => src.TotalSaved))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.StartDate <= DateTime.Now && src.EndDate >= DateTime.Now))
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest => dest.CompanyLogoURL, opt => opt.MapFrom(src => src.Company.CompanyLogoURL))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.Genre.GenreName))
                .ForMember(dest => dest.CompanyID, opt => opt.MapFrom(src=>src.CompanyID))
                .ForMember(dest => dest.genreID, opt => opt.MapFrom(src => src.GenreID))
                .ReverseMap();
        }
    }
}
