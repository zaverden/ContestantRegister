using ContestantRegister.Models;

namespace ContestantRegister.ViewModels.ListItemViewModels
{
    public class UserListItemViewModel
    {
        public string Id { get; set; }
        public string DisplayName { get; set; }
        public UserType UserType { get; set; }


        public class Profile : AutoMapper.Profile
        {
            public Profile()
            {
                CreateMap<ApplicationUser, UserListItemViewModel>()
                    .ForMember(ulivm => ulivm.DisplayName, opt => opt.MapFrom(au => $"{au.Name} ({au.Email})"));
            }
        }
    }
}
