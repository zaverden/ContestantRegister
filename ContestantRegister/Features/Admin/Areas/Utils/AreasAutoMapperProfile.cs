using AutoMapper;
using ContestantRegister.Controllers.Areas.ViewModels;
using ContestantRegister.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers
{
    public class AreasAutoMapperProfile : Profile
    {
        public AreasAutoMapperProfile()
        {
            CreateMap<Area, AreaViewModel>();
            CreateMap<AreaViewModel, Area>();
        }
    }
}

