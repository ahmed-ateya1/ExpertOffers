using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.GenreCouponDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class GenreCouponConfig : Profile
    {
        public GenreCouponConfig()
        {
            CreateMap<GenreCouponAddRequest,GenreCoupon>()
                .ForMember(dest=>dest.GenreID , opt=>opt.MapFrom(src => Guid.NewGuid()))
                .ReverseMap();
            CreateMap<GenreCouponUpdateRequest, GenreCoupon>().ReverseMap();
            CreateMap<GenreCoupon, GenreCouponResponse>().ReverseMap();
        }
    }
}
