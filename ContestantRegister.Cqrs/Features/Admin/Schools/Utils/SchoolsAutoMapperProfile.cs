using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Schools.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Schools.Utils
{
    public class SchoolsAutoMapperProfile : Profile
    {
        public SchoolsAutoMapperProfile()
        {
            CreateMap<School, School>();
            CreateMap<School, SchoolListItemViewModel>()
                .ForMember(x => x.City, opt => opt.MapFrom(y => y.City.Name));
        }
    }
}
