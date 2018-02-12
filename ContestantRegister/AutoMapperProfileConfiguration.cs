using AutoMapper;
using ContestantRegister.Models;
using ContestantRegister.Models.AccountViewModels;

namespace ContestantRegister
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<City, City>();
            CreateMap<School, School>();
            CreateMap<Institution, Institution>();

            CreateMap<RegisterViewModel, Pupil>();
            CreateMap<RegisterViewModel, Student>();
            CreateMap<RegisterViewModel, Trainer>();
        }
    }
}
