using AutoMapper;
using AFRI_AusCare.DataModels;
using AFRI_AusCare.Models;

namespace AFRI_AusCare.Configurations
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<GalleryModel, Gallery>().ReverseMap();
            CreateMap<TeamModel, Team>().ReverseMap();
            CreateMap<BoardMemberModel, BoardMember>().ReverseMap();
            CreateMap<KeyPartnerModel, KeyPartner>().ReverseMap();
        }
    }
}
