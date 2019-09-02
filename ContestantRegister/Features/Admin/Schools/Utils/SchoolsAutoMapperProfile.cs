using AutoMapper;
using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers.Schools
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
