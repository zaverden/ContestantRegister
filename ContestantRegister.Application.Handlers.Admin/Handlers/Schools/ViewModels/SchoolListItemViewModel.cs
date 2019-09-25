using ContestantRegister.Cqrs.Features._Common.ListViewModel;

namespace ContestantRegister.Cqrs.Features.Admin.Schools.ViewModels
{
    public class SchoolListItemViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string ShortName { get; set;}

        public string FullName { get; set; }

        public string City { get; set; }

        public string Email { get; set; }

        public string Site { get; set; }
    }
}
