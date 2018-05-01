using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Models;
using ContestantRegister.Properties;

namespace ContestantRegister.ViewModels.HomeViewModels
{
    public class EditIndividualContestRegistrationViewModel : IndividualContestRegistrationViewModel
    {
        [Display(Name = "Логин в ЯКонтесте")]
        public string YaContestLogin { get; set; }

        [Display(Name = "Пароль в ЯКонтесте")]
        public string YaContestPassword { get; set; }

        [Display(Name = "Data регистрации")]
        public DateTime? RegistrationDateTime { get; set; }

        [Display(Name = "Кем зарегистрирован")]
        public string RegistredByName { get; set; }

        [Display(Name = "Участник")]
        public string ParticipantName { get; set; }

        [Display(Name = "№")]
        [Range(1, int.MaxValue)]
        public int Number { get; set; }
    }

    public class CreateIndividualContestRegistrationViewModel : IndividualContestRegistrationViewModel
    {
    }

    public abstract class IndividualContestRegistrationViewModel
    {
        [Display(Name = "Участник")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string Participant1Id { get; set; }

        [Display(Name = "Тренер")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public string TrainerId { get; set; }

        [Display(Name = "Руководитель")]
        public string ManagerId { get; set; }

        [Display(Name = "Учебное заведение")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int StudyPlaceId { get; set; }

        public bool IsProgrammingLanguageNeeded { get; set; }

        [Display(Name = "Язык программирования")]
        [MaxLength(100)]
        public string ProgrammingLanguage { get; set; }

        [Display(Name = "Город")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        public int CityId { get; set; }

        public bool IsAreaRequired { get; set; }

        [Display(Name = "Площадка")]
        public string Area { get; set; }

        [Display(Name = "Рабочее место")]
        [MaxLength(50)]
        public string ComputerName { get; set; }

        [Display(Name = "Курс")]
        [Range(1, 6)]
        public int? Course { get; set; }

        [Display(Name = "Класс")]
        [Range(1, 11)]
        public int? Class { get; set; }

        [Display(Name = "Категория")]
        public StudentType? StudentType { get; set; }

        public ParticipantType ParticipantType { get; set; }

        public int RegistrationId { get; set; }
        
        public string ContestName { get; set; }

        public int ContestId { get; set; }
        
    }
}
