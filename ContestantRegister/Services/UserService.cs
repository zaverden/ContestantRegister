using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Models.AccountViewModels;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Services
{
    public interface IUserService
    {
        bool IsPupil(ClaimsPrincipal user);
        bool IsStudent(ClaimsPrincipal user);
        bool IsTrainer(ClaimsPrincipal user);

        bool IsPupil(ContestantUser user);
        bool IsStudent(ContestantUser user);
        bool IsTrainer(ContestantUser user);

        Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user);

        Task<List<KeyValuePair<string, string>>> ValidateUserAsync(UserViewModelBase viewModel);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public bool IsPupil(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new InvalidOperationException("User not authentificated");

            return _context.Users.OfType<Pupil>().Any(u => u.UserName == user.Identity.Name);
        }

        public bool IsStudent(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new InvalidOperationException("User not authentificated");

            return _context.Users.OfType<Student>().Any(u => u.UserName == user.Identity.Name);
        }

        public bool IsTrainer(ClaimsPrincipal user)
        {
            if (!user.Identity.IsAuthenticated) throw new InvalidOperationException("User not authentificated");

            return _context.Users.OfType<Trainer>().Any(u => u.UserName == user.Identity.Name);
        }

        public bool IsPupil(ContestantUser user)
        {
            return user is Pupil;
        }

        public bool IsStudent(ContestantUser user)
        {
            return user is Student;
        }

        public bool IsTrainer(ContestantUser user)
        {
            return user is Trainer;
        }

        public async Task<ApplicationUser> GetCurrentUserAsync(ClaimsPrincipal user)
        {
            return await _context.Users.SingleAsync(u => u.UserName == user.Identity.Name);
        }

        public async Task<List<KeyValuePair<string, string>>> ValidateUserAsync(UserViewModelBase viewModel)
        {
            var result = new List<KeyValuePair<string, string>>();

            var studyPlace = await _context.StudyPlaces.SingleAsync(s => s.Id == viewModel.StudyPlaceId);
            switch (viewModel.UserType)
            {
                case UserType.Pupil:
                    if (studyPlace is Institution)
                        result.Add(KeyValuePair.Create(nameof(viewModel.StudyPlaceId), "У школьника не может быть указан вуз в качестве учебного заведения"));
                    break;
                case UserType.Student:
                    if (studyPlace is School)
                        result.Add(KeyValuePair.Create(nameof(viewModel.StudyPlaceId), "У студента не может быть указана школа в качестве учебного заведения"));
                    if (!viewModel.EducationStartDate.HasValue)
                        result.Add(KeyValuePair.Create(nameof(viewModel.EducationStartDate), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.EducationStartDate))));
                    if (!viewModel.EducationEndDate.HasValue)
                        result.Add(KeyValuePair.Create(nameof(viewModel.EducationEndDate), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.EducationEndDate))));
                    if (!viewModel.DateOfBirth.HasValue)
                        result.Add(KeyValuePair.Create(nameof(viewModel.DateOfBirth), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.DateOfBirth))));
                    if (viewModel.EducationStartDate > viewModel.EducationEndDate)
                        result.Add(KeyValuePair.Create(string.Empty, "Дата начала обучения должна быть позже даты конца обучения"));
                    if (DateTime.Now.Year - viewModel.DateOfBirth.Value.Year < 16)
                        result.Add(KeyValuePair.Create(nameof(viewModel.DateOfBirth), "Возраст слишком маленький, чтобы быть студентом"));
                    break;
            }

            if (viewModel.UserType == UserType.Student || viewModel.UserType == UserType.Trainer)
            {
                if (string.IsNullOrEmpty(viewModel.FirstName))
                    result.Add(KeyValuePair.Create(nameof(viewModel.FirstName), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.FirstName))));
                if (string.IsNullOrEmpty(viewModel.LastName))
                    result.Add(KeyValuePair.Create(nameof(viewModel.LastName), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.LastName))));
            }

            if (viewModel.UserType == UserType.Trainer)
            {
                if (string.IsNullOrEmpty(viewModel.PhoneNumber))
                    result.Add(KeyValuePair.Create(nameof(viewModel.PhoneNumber), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.PhoneNumber))));
            }

            return result;
        }
    }
}
