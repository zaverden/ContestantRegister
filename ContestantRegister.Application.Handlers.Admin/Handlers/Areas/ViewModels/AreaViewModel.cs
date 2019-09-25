using ContestantRegister.Cqrs.Features._Common.ListViewModel;

namespace ContestantRegister.Cqrs.Features.Admin.Areas.ViewModels
{
    public class AreaViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string Name { get; set; }
    }
}
