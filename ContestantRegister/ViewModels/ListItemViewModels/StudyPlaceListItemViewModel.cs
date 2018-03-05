using ContestantRegister.Models;

namespace ContestantRegister.ViewModels.ListItemViewModels
{
    public class StudyPlaceListItemViewModel
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }

        public class Profile : AutoMapper.Profile
        {
            public Profile()
            {
                CreateMap<StudyPlace, StudyPlaceListItemViewModel>()
                    .ForMember(splivm => splivm.Type, opt => opt.MapFrom(sp => sp.GetType().Name));
            }
        }
    }
}
