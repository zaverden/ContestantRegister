using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ContestantRegister.Data;
using ContestantRegister.Models;
using ContestantRegister.Utils;
using ContestantRegister.ViewModels.Contest.Registration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Logical;

namespace ContestantRegister.Services
{
    public interface IContestRegistrationService
    {
        Task<List<KeyValuePair<string, string>>> ValidateCreateIndividualContestRegistrationAsync(CreateIndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user);
        Task<List<KeyValuePair<string, string>>> ValidateEditIndividualContestRegistrationAsync(EditIndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user);

        Task<List<KeyValuePair<string, string>>> ValidateCreateTeamContestRegistrationAsync(CreateTeamContestRegistrationViewModel viewModel, ClaimsPrincipal user);
        Task<List<KeyValuePair<string, string>>> ValidateEditTeamContestRegistrationAsync(EditTeamContestRegistrationViewModel viewModel, ClaimsPrincipal user);
    }

    public class ContestRegistrationService : IContestRegistrationService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ContestRegistrationService(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        private async Task<List<KeyValuePair<string, string>>> ValidateIndividualContestRegistrationAsync(IndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user, bool editRegistration)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (editRegistration && !viewModel.Number.HasValue)
                result.Add(KeyValuePair.Create(nameof(viewModel.Number), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.Number))));

            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

            if (contest.IsProgrammingLanguageNeeded && string.IsNullOrEmpty(viewModel.ProgrammingLanguage))
                result.Add(KeyValuePair.Create(nameof(viewModel.ProgrammingLanguage), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.ProgrammingLanguage))));

            var studyPlace = await _context.StudyPlaces.SingleAsync(s => s.Id == viewModel.StudyPlaceId);
            if (!viewModel.IsOutOfCompetition &&
                    (contest.ParticipantType == ParticipantType.Pupil && studyPlace is Institution ||
                     contest.ParticipantType == ParticipantType.Student && studyPlace is School)
                )
                result.Add(KeyValuePair.Create(nameof(viewModel.StudyPlaceId), "Тип учебного заведения не соответствует типу контеста"));
            if (viewModel.CityId != studyPlace.CityId)
                result.Add(KeyValuePair.Create(nameof(viewModel.CityId), "Выбранный город не соответствует городу учебного заведения"));

            if (editRegistration)
            {
                var dbRegistration = await _context.ContestRegistrations.Include(r => r.Participant1).SingleAsync(r => r.Id == viewModel.RegistrationId);
                if (dbRegistration.Participant1Id != viewModel.Participant1Id)
                    result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Нельзя менять пользователя при изменении регистрации"));
            }
            else
            {
                var participantRegistred = await _context.ContestRegistrations.AnyAsync(r => r.ContestId == contest.Id && r.Participant1Id == viewModel.Participant1Id);
                if (participantRegistred) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Указанный участник уже зарегистрирован в контесте"));
            }

            if (viewModel.Participant1Id == viewModel.TrainerId) result.Add(KeyValuePair.Create(nameof(viewModel.TrainerId), "Участник не может быть своим тренером"));
            if (viewModel.Participant1Id == viewModel.ManagerId) result.Add(KeyValuePair.Create(nameof(viewModel.ManagerId), "Участник не может быть своим руководителем"));

            var currentUser = await _userManager.GetUserAsync(user);
            if (!user.IsInRole(Roles.Admin) && viewModel.Participant1Id != currentUser.Id && viewModel.TrainerId != currentUser.Id && viewModel.ManagerId != currentUser.Id)
                result.Add(KeyValuePair.Create(string.Empty, "Вы должны быть участником, тренером или руководителем, чтобы завершить регистрацию"));

            var participant = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant1Id);
            var trainer = await _context.Users.SingleAsync(u => u.Id == viewModel.TrainerId);
            var manager = await _context.Users.SingleOrDefaultAsync(u => u.Id == viewModel.ManagerId);

            if (contest.ParticipantType == ParticipantType.Pupil && !viewModel.IsOutOfCompetition)
            {
                if (participant.UserType != UserType.Pupil)
                {
                    result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Только школьник может быть участником школьного контеста"));
                }
                if (!viewModel.Class.HasValue)
                {
                    result.Add(KeyValuePair.Create(nameof(viewModel.Class), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.Class))));
                }
            }

            if (contest.ParticipantType == ParticipantType.Student && !viewModel.IsOutOfCompetition)
            {
                if (participant.UserType != UserType.Student) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Только студент может быть участником студенческого контеста"));
                if (trainer.UserType == UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.TrainerId), "Школьник не может быть тренером на студенческом контесте"));
                if (manager != null && manager.UserType == UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.ManagerId), "Школьник не может быть руководителем на студенческом контесте"));

                if (!viewModel.Course.HasValue)
                    result.Add(KeyValuePair.Create(nameof(viewModel.Course), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.Course))));
            }

            if (contest.IsAreaRequired && !viewModel.ContestAreaId.HasValue)
                result.Add(KeyValuePair.Create(nameof(viewModel.ContestAreaId), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.ContestAreaId))));

            return result;
        }

        public Task<List<KeyValuePair<string, string>>> ValidateCreateIndividualContestRegistrationAsync(CreateIndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user)
        {
            return ValidateIndividualContestRegistrationAsync(viewModel, user, false);
        }

        public Task<List<KeyValuePair<string, string>>> ValidateEditIndividualContestRegistrationAsync(EditIndividualContestRegistrationViewModel viewModel, ClaimsPrincipal user)
        {
            return ValidateIndividualContestRegistrationAsync(viewModel, user, true);
        }

        public Task<List<KeyValuePair<string, string>>> ValidateCreateTeamContestRegistrationAsync(CreateTeamContestRegistrationViewModel viewModel, ClaimsPrincipal user)
        {
            return ValidateTeamContestRegistrationAsync(viewModel, user, false);
        }

        public Task<List<KeyValuePair<string, string>>> ValidateEditTeamContestRegistrationAsync(EditTeamContestRegistrationViewModel viewModel, ClaimsPrincipal user)
        {
            return ValidateTeamContestRegistrationAsync(viewModel, user, true);
        }

        private async Task<List<KeyValuePair<string, string>>> ValidateTeamContestRegistrationAsync(TeamContestRegistrationViewModel viewModel, ClaimsPrincipal user, bool editRegistration)
        {
            var result = new List<KeyValuePair<string, string>>();

            if (editRegistration && !viewModel.Number.HasValue)
                result.Add(KeyValuePair.Create(nameof(viewModel.Number), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.Number))));

            var contest = await _context.Contests.SingleAsync(c => c.Id == viewModel.ContestId);

            if (contest.IsProgrammingLanguageNeeded && string.IsNullOrEmpty(viewModel.ProgrammingLanguage))
                result.Add(KeyValuePair.Create(nameof(viewModel.ProgrammingLanguage), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.ProgrammingLanguage))));

            var studyPlace = await _context.StudyPlaces.SingleAsync(s => s.Id == viewModel.StudyPlaceId);
            if (!viewModel.IsOutOfCompetition && 
                    (contest.ParticipantType == ParticipantType.Pupil && studyPlace is Institution ||
                     contest.ParticipantType == ParticipantType.Student && studyPlace is School) 
                )
                result.Add(KeyValuePair.Create(nameof(viewModel.StudyPlaceId), "Тип учебного заведения не соответствует типу контеста"));
            if (viewModel.CityId != studyPlace.CityId)
                result.Add(KeyValuePair.Create(nameof(viewModel.CityId), "Выбранный город не соответствует городу учебного заведения"));

            var participant1Registred = await ParticitantExistsInOtherTeams(viewModel.RegistrationId, contest, viewModel.Participant1Id);
            if (participant1Registred) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Участник уже зарегистрирован в другой команде"));

            var participant2Registred = await ParticitantExistsInOtherTeams(viewModel.RegistrationId, contest, viewModel.Participant2Id);
            if (participant2Registred) result.Add(KeyValuePair.Create(nameof(viewModel.Participant2Id), "Участник уже зарегистрирован в другой команде"));

            var participant3Registred = await ParticitantExistsInOtherTeams(viewModel.RegistrationId, contest, viewModel.Participant3Id);
            if (participant3Registred) result.Add(KeyValuePair.Create(nameof(viewModel.Participant3Id), "Участник уже зарегистрирован в другой команде"));

            if (viewModel.Participant1Id == viewModel.Participant2Id)
            {
                result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Участник указан дважды"));
                result.Add(KeyValuePair.Create(nameof(viewModel.Participant2Id), "Участник указан дважды"));
            }
            if (viewModel.Participant1Id == viewModel.Participant3Id)
            {
                result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), "Участник указан дважды"));
                result.Add(KeyValuePair.Create(nameof(viewModel.Participant3Id), "Участник указан дважды"));
            }
            if (viewModel.Participant2Id == viewModel.Participant3Id)
            {
                result.Add(KeyValuePair.Create(nameof(viewModel.Participant2Id), "Участник указан дважды"));
                result.Add(KeyValuePair.Create(nameof(viewModel.Participant3Id), "Участник указан дважды"));
            }

            if (viewModel.Participant1Id == viewModel.TrainerId ||
                viewModel.Participant2Id == viewModel.TrainerId ||
                viewModel.Participant3Id == viewModel.TrainerId)
            {
                result.Add(KeyValuePair.Create(nameof(viewModel.TrainerId), "Участник не может быть тренером"));
            }

            if (viewModel.Participant1Id == viewModel.ManagerId ||
                viewModel.Participant2Id == viewModel.ManagerId ||
                viewModel.Participant3Id == viewModel.ManagerId)
            {
                result.Add(KeyValuePair.Create(nameof(viewModel.ManagerId), "Участник не может быть руководителем"));
            }

            var currentUser = await _userManager.GetUserAsync(user);
            if (!user.IsInRole(Roles.Admin) && 
                viewModel.Participant1Id != currentUser.Id &&
                viewModel.Participant2Id != currentUser.Id &&
                viewModel.Participant3Id != currentUser.Id &&
                viewModel.TrainerId != currentUser.Id && 
                viewModel.ManagerId != currentUser.Id)
            {
                result.Add(KeyValuePair.Create(string.Empty, "Вы должны быть участником, тренером или руководителем, чтобы завершить регистрацию"));
            }

            var participant1 = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant1Id);
            var participant2 = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant2Id);
            var participant3 = await _context.Users.SingleAsync(u => u.Id == viewModel.Participant3Id);
            var trainer = await _context.Users.SingleAsync(u => u.Id == viewModel.TrainerId);
            var manager = await _context.Users.SingleOrDefaultAsync(u => u.Id == viewModel.ManagerId);

            if (contest.ParticipantType == ParticipantType.Pupil && !viewModel.IsOutOfCompetition)
            {
                var message = "Только школьник может быть участником школьного контеста";
                if (participant1.UserType != UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), message));
                if (participant2.UserType != UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.Participant2Id), message));
                if (participant3.UserType != UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.Participant3Id), message));                
            }

            if (contest.ParticipantType == ParticipantType.Student && !viewModel.IsOutOfCompetition)
            {
                var message = "Чтобы зарегистрировать участника - не студента, поставьте флаг 'Вне конкурса'";
                if (participant1.UserType != UserType.Student) result.Add(KeyValuePair.Create(nameof(viewModel.Participant1Id), message));
                if (participant2.UserType != UserType.Student) result.Add(KeyValuePair.Create(nameof(viewModel.Participant2Id), message));
                if (participant3.UserType != UserType.Student) result.Add(KeyValuePair.Create(nameof(viewModel.Participant3Id), message));
                if (trainer.UserType == UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.TrainerId), "Школьник не может быть тренером на студенческом контесте"));
                if (manager != null && manager.UserType == UserType.Pupil) result.Add(KeyValuePair.Create(nameof(viewModel.ManagerId), "Школьник не может быть руководителем на студенческом контесте"));
            }

            if (contest.IsAreaRequired && !viewModel.ContestAreaId.HasValue)
                result.Add(KeyValuePair.Create(nameof(viewModel.ContestAreaId), viewModel.GetRequredFieldErrorMessage(nameof(viewModel.ContestAreaId))));

            return result;
        }

        private Task<bool> ParticitantExistsInOtherTeams(int registrationId, Contest contest, string participantId)
        {
            return _context.TeamContestRegistrations.AnyAsync(r =>
                r.Id != registrationId &&
                r.ContestId == contest.Id &&
                r.Status == ContestRegistrationStatus.Completed &&
                (r.Participant1Id == participantId ||
                 r.Participant2Id == participantId ||
                 r.Participant3Id == participantId));
        }
    }
}
