using ContestantRegister.Cqrs.Features._Common.ListViewModel;

namespace ContestantRegister.Cqrs.Features.Admin.Cities.ViewModels
{
    public class CityListItemViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string Name { get; set; }

        public string Region { get; set; }
    }
}
