using AutoMapper;
using ContestantRegister.Models;

namespace ContestantRegister
{
    public class AutoMapperProfileConfiguration : Profile
    {
        public AutoMapperProfileConfiguration()
        {
            CreateMap<City, City>();
            CreateMap<School, School>();
            CreateMap<Institution, Institution>();
        }
    }
}
