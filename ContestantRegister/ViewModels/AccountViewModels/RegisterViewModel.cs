using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Properties;

namespace ContestantRegister.Models.AccountViewModels
{
    public class RegisterViewModel : UserWithCredViewModelBase
    {
    }

    public abstract class UserWithCredViewModelBase : UserViewModelBase
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [StringLength(100, ErrorMessage = "{0} должен быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Подтверждение пароля")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

    }

    public abstract class UserViewModelBase
    {
        [Required (ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Email подтвержден")]
        public bool EmailConfirmed { get; set; }

        [Display(Name = "Роль")]
        public UserType UserType { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Имя")]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Фамилия")]
        public string Surname { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [MaxLength(50)]
        [Display(Name = "Отчество")]
        public string Patronymic { get; set; }

        /// <summary>
        /// Поле необязательно для школьника итренера школьников, но обязаьельно для студента и тренера студентов
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        /// <summary>
        /// Поле необязательно для школьника итренера школьников, но обязаьельно для студента и тренера студентов
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Город")]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Место учебы")]
        public int StudyPlaceId { get; set; }


        //Student
        [Display(Name = "Дата начала обучения")]
        public DateTime? EducationStartDate { get; set; }

        [Display(Name = "Дата завершения обучения")]
        public DateTime? EducationEndDate { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime? DateOfBirth { get; set; }

        //Trainer
        [Display(Name = "Номер мобильного телефона")]
        public string PhoneNumber { get; set; }

    }

    public enum UserType : int
    {
        [Display(Name = "Школьник")]
        Pupil = 1,

        [Display(Name = "Студент")]
        Student = 2,

        [Display(Name = "Тренер")]
        Trainer = 3,
    }
}
