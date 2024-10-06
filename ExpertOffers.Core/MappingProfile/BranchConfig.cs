using AutoMapper;
using ExpertOffers.Core.Domain.Entities;
using ExpertOffers.Core.Dtos.BranchDto;

namespace ExpertOffers.Core.MappingProfile
{
    public class BranchConfig : Profile
    {
        public BranchConfig()
        {

            CreateMap<BranchAddRequest, Branch>()
                .ForMember(dest => dest.BranchID, opt => opt.Ignore());


            CreateMap<BranchUpdateRequest, Branch>();

            CreateMap<Branch, BranchResponse>()
                .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Company.CompanyName))
                .ForMember(dest=>dest.CompanyLogoURL , opt=>opt.MapFrom(src=>src.Company.CompanyLogoURL));
        }
    }
}
