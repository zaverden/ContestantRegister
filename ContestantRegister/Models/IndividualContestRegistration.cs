using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class IndividualContestRegistration : ContestRegistration
    {
        [Display(Name = "Курс")]
        [Range(1, 6)]
        public int? Course { get; set; }

        [Display(Name = "Класс")]
        [Range(1, 11)]
        public int? Class { get; set; }

        [Display(Name = "Категория")]
        public StudentType? StudentType { get; set; }


    }
}
