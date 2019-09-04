using AutoMapper;
using ContestantRegister.Cqrs.Features.Admin.Institutions.ViewModels;
using ContestantRegister.Models;

namespace ContestantRegister.Cqrs.Features.Admin.Institutions.Utils
{
    public class InstitutionsAutoMapperProfile : Profile
    {
        public InstitutionsAutoMapperProfile()
        {
            CreateMap<Institution, Institution>();
            CreateMap<Institution, InstitutionListItemViewModel>()
                .ForMember(x => x.City, opt => opt.MapFrom(y => y.City.Name));
        }
    }
}
