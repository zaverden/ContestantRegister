using ContestantRegister.Cqrs.Features._Common.ListViewModel;

namespace ContestantRegister.Cqrs.Features.Admin.Regions.ViewModels
{
    public class RegionViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string Name { get; set; }
    }
}
