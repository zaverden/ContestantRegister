﻿using System;
using System.ComponentModel.DataAnnotations;
using ContestantRegister.Domain.Properties;
using ContestantRegister.Models;
using ContestantRegister.Services.DomainServices;

namespace ContestantRegister.Application.Handlers.Common.Handlers.Shared.ViewModels
{
    public abstract class UserViewModelBase : IApplicationUser
    {
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmailErrorMessage")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Email подтвержден")]
        public bool EmailConfirmed { get; set; }

        [EmailAddress(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidEmailErrorMessage")]
        [Display(Name = "Baylor email")]
        [MaxLength(100)]
        public string BaylorEmail { get; set; }

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
        /// Поле необязательно для школьника и тренера школьников, но обязаьельно для студента и тренера студентов
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "First name")]
        [RegularExpression(@"^[a-zA-Z]{0,50}$", ErrorMessage = "Допустимы заглавные и строчные английские буквы")]
        public string FirstName { get; set; }

        /// <summary>
        /// Поле необязательно для школьника и тренера школьников, но обязательно для студента и тренера студентов
        /// </summary>
        [MaxLength(50)]
        [Display(Name = "Last name")]
        [RegularExpression(@"^[a-zA-Z]{0,50}$", ErrorMessage = "Допустимы заглавные и строчные английские буквы")]
        public string LastName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Город")]
        public int CityId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [Display(Name = "Учебное заведение")]
        public int StudyPlaceId { get; set; }

        [Display(Name = "Дата рождения")]
        [Required(ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "RequiredFieldErrorMessage")]
        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        [Display(Name = "Дата начала обучения")]
        [DataType(DataType.Date)]
        public DateTime? EducationStartDate { get; set; }

        //Student
        [Display(Name = "Дата завершения обучения")]
        [DataType(DataType.Date)]
        public DateTime? EducationEndDate { get; set; }

        [Display(Name = "Ссылка на профиль вконтакте")]
        [MaxLength(100, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "MaxlenFieldErrorMessage")]
        [DataType(DataType.Url, ErrorMessageResourceType = typeof(Resource), ErrorMessageResourceName = "InvalidUrlErrorMessage")]
        public string VkProfile { get; set; }

        [Display(Name = "Регистрация на бейлоре завершена")]
        public bool IsBaylorRegistrationCompleted { get; set; }

        // для студента обязательное поле
        [Display(Name = "Номер мобильного телефона")]
        public string PhoneNumber { get; set; }

        //обязательное для студента 
        [Display(Name = "Категория")]
        public StudentType? StudentType { get; set; }

        //Trainer

        public bool IsUserTypeDisabled { get; set; }

        public bool CanSuggestStudyPlace { get; set; }
    }
}
