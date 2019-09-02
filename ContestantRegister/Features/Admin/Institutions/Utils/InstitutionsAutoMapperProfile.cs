using AutoMapper;
using ContestantRegister.Controllers.Institutions.ViewModels;
using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers
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
