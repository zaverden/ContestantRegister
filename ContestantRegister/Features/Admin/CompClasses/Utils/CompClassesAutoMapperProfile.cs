using AutoMapper;
using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.CompClasses
{
    public class CompClassesAutoMapperProfile : Profile
    {
        public CompClassesAutoMapperProfile()
        {
            CreateMap<CompClass, CompClassListItemViewModel>()
                .ForMember(x => x.Area, opt => opt.MapFrom(y => y.Area.Name));
        }
    }
}
