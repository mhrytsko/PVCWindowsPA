using AutoMapper;

namespace webapi.Framework
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User
            CreateMap<Models.User, ViewModels.UserData>()
                .ReverseMap()
                .ForMember(dest => dest.PersonalDataId, opt => opt.MapFrom((source, destination) => 
                {
                    return source.PersonalDetail?.Id;
                }));

        }
    }
}
