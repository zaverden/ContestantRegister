using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.ViewModels;
using ContestantRegister.ViewModels.HomeViewModels;
using Microsoft.EntityFrameworkCore;

namespace ContestantRegister.Services
{
    public interface IContestRegistrationService
    {
        Task<List<KeyValuePair<string, string>>> ValidateContestRegistrationAsync(IndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user, bool editRegistration);
    }

    public class ContestRegistrationService : IContestRegistrationService
    {
        private readonly ApplicationDbContext _context;

        public ContestRegistrationService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<KeyValuePair<string, string>>> ValidateContestRegistrationAsync(IndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user, bool editRegistration)
        {
            var result = new List<KeyValuePair<string, string>>();

            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

            if (contest.IsProgrammingLanguageNeeded && string.IsNullOrEmpty(viewModel.ProgrammingLanguage))
                result.Add(KeyValuePair.Create(nameof(viewModel.ProgrammingLanguage), "Поле \"Язык программирования\" обязательное"));

            var studyPlace = await _context.StudyPlaces.OfType<StudyPlace>().SingleAsync(s => s.Id == viewModel.StudyPlaceId);
            if (contest.ParticipantType == ParticipantType.Pupil && studyPlace is Institution ||
                contest.ParticipantType == ParticipantType.Student && studyPlace is School)
                result.Add(KeyValuePair.Create(nameof(viewModel.StudyPlaceId), "Тип учебного заведения не соответствует типу контеста"));
            if (viewModel.CityId != studyPlace.CityId)
                result.Add(KeyValuePair.Create(nameof(viewModel.CityId), "Выбранный город не соответствует городу учебного заведения"));

            if (editRegistration)
            {
                var dbRegistration = await _context.ContestRegistrations.SingleAsync(r => r.Id == viewModel.RegistrationId);
                if (dbRegistration.Participant1Id != viewModel.Participant1Id)
                    result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Нельзя менять пользователя при изменении регистрации"));
            }
            else
            {
                var participanrRegistred = await _context.ContestRegistrations.AnyAsync(r => r.ContestId == contest.Id && r.Participant1Id == viewModel.Participant1Id);
                if (participanrRegistred) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Указанный участник уже зарегистрирован в контесте"));
            }

            if (viewModel.Participant1Id == viewModel.TrainerId) result.Add(KeyValuePair.Create(nameof(viewModel.TrainerId), "Участник не может быть своим тренером"));
            if (viewModel.Participant1Id == viewModel.ManagerId) result.Add(KeyValuePair.Create(nameof(viewModel.ManagerId), "Участник не может быть своим руководителем"));

            var currentUser = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.UserName == user.Identity.Name);
            if (!user.IsInRole(Roles.Admin) && viewModel.Participant1Id != currentUser.Id && viewModel.TrainerId != currentUser.Id && viewModel.ManagerId != currentUser.Id)
                result.Add(KeyValuePair.Create(string.Empty, "Вы должны быть участником, тренером или руководителем чтобы завершить регистрацию"));

            var participant = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.Id == viewModel.Participant1Id);
            var trainer = await _context.Users.OfType<ContestantUser>().SingleAsync(u => u.Id == viewModel.TrainerId);
            var manager = await _context.Users.OfType<ContestantUser>().SingleOrDefaultAsync(u => u.Id == viewModel.ManagerId);

            if (contest.ParticipantType == ParticipantType.Pupil)
            {
                if (!(participant is Pupil))
                {
                    result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Только школьник может быть участником школьного контеста"));
                }
            }

            if (contest.ParticipantType == ParticipantType.Student)
            {
                if (!(participant is Student)) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Только студент может быть участником студенческого контеста"));
                if (trainer is Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.TrainerId), "Школьник не может быть тренером на студенческом контесте"));
                if (manager is Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.ManagerId), "Школьник не может быть руководителем на студенческом контесте"));
            }

            if (contest.IsAreaRequired && string.IsNullOrEmpty(viewModel.Area))
                result.Add(KeyValuePair.Create(nameof(viewModel.Area), "Поле 'Площадка' обязательное"));

            return result;
        }
    }
}
