using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BulletinGenreDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExpertOffers.Core.MappingProfile
{
    public class BulletinGenreConfig : Profile
    {
        public BulletinGenreConfig()
        {
            CreateMap<BulletinGenreAddRequest, BulletinGenre>()
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => Guid.NewGuid()))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.GenreName))
                .ReverseMap();

            CreateMap<BulletinGenreUpdateRequest, BulletinGenre>()
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.GenreName))
                .ReverseMap();

            CreateMap<BulletinGenre, BulletinGenreResponse>()
                .ForMember(dest => dest.GenreID, opt => opt.MapFrom(src => src.GenreID))
                .ForMember(dest => dest.GenreName, opt => opt.MapFrom(src => src.GenreName))
                .ReverseMap();


        }
    }
}
