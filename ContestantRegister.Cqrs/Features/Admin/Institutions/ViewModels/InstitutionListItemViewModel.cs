using ContestantRegister.Cqrs.Features._Common.ListViewModel;

namespace ContestantRegister.Cqrs.Features.Admin.Institutions.ViewModels
{
    public class InstitutionListItemViewModel
    {
        public int Id { get; set; }

        [OrderBy]
        public string ShortName { get; set; }

        public string FullName { get; set; }

        public string City { get; set; }

        public string ShortNameEnglish { get; set; }

        public string FullNameEnglish { get; set; }

        public string BaylorFullName { get; set; }

        public string Site { get; set; }
    }
}


