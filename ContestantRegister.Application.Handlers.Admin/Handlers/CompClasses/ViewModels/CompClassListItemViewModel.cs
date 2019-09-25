using ContestantRegister.Cqrs.Features._Common.ListViewModel;

namespace ContestantRegister.Cqrs.Features.Admin.CompClasses.ViewModels
{
    public class CompClassListItemViewModel 
    {
        public int Id { get; set; }
        [OrderBy]
        public string Name { get; set;}
        public string Area { get; set; }

        public string CompNumber { get; set; }
        public string Comment { get; set; }
    }
}
