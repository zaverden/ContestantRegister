using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.HomeViewModels
{
    public class ImportParticipantsViewModel
    {
        public string ContestName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Csv данные")]
        public string Data { get; set; }
    }
}
