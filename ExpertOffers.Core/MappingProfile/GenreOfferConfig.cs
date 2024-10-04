using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreOffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class GenreOfferConfig : Profile
    {
        public GenreOfferConfig()
        {
            CreateMap<GenreAddRequest, GenreOffer>()
                .ReverseMap();
            CreateMap<GenreUpdateRequest, GenreOffer>()
                .ReverseMap();
            CreateMap<GenreOffer, GenreResponse>()
                .ForMember(dest=>dest.GenreOfferImgURL, opt => opt.MapFrom(src => src.GenreImgURL))
                .ReverseMap();
        }
    }
}
