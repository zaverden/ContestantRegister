using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models
{
    [DisplayName("Школа")]
    public class School : StudyPlace
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [EmailAddress]
        public string Email { get; set; }
    }
}
