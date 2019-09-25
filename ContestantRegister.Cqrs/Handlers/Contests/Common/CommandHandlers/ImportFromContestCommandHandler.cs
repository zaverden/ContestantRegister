using System;
using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features._Common.CommandHandlers;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Commands;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Utils;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Exceptions;
using ContestantRegister.Services.Extensions;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers
{
    public class ImportFromContestCommandHandler : RepositoryCommandBaseHandler<ImportFromContestCommand>
    {
        public ImportFromContestCommandHandler(IRepository repository) : base(repository)
        {
        }

        public override async Task HandleAsync(ImportFromContestCommand command)
        {
            var contest = await Repository.Set<Contest>().SingleOrDefaultAsync(c => c.Id == command.ContestId);
            if (contest == null) throw new EntityNotFoundException();
            
            var loginsForImport = command.ViewModel.ParticipantYaContestLogins.SplitByNewLineEndAndRemoveWindowsLineEnds().ToHashSet();
            var accounts = contest.YaContestAccountsCSV.SplitByNewLineEndAndRemoveWindowsLineEnds();
            var registrations = Repository.Set<ContestRegistration>().Where(r => r.ContestId == command.ViewModel.FromContestId);
            foreach (var registration in registrations)
            {
                if (loginsForImport.Contains(registration.YaContestLogin))
                {
                    if (contest.UsedAccountsCount == accounts.Length)
                    {
                        throw new InvalidOperationException("В контесте, в который импортируются участники, не хватает яконтест аккаунтов для завершения импорта");
                    }

                    var account = accounts[contest.UsedAccountsCount].Split(',');
                    //Здесь не нужно выставлять время регистрации и зарегистрировавшего, т.к. эти данные подставляются при подтверждении регистрации

                    var newRegistration = CreateContestRegistrationForImportFromContest(registration);
                    newRegistration.Status = ContestRegistrationStatus.NotCompleted;
                    newRegistration.ProgrammingLanguage = registration.ProgrammingLanguage;
                    newRegistration.Participant1Id = registration.Participant1Id;
                    newRegistration.TrainerId = registration.TrainerId;
                    newRegistration.ManagerId = registration.ManagerId;
                    newRegistration.StudyPlaceId = registration.StudyPlaceId;
                    newRegistration.ContestId = command.ContestId;
                    newRegistration.YaContestLogin = account[0];
                    newRegistration.YaContestPassword = account[1];
                    newRegistration.Number = contest.RegistrationsCount + 1;

                    contest.UsedAccountsCount++;
                    contest.RegistrationsCount++;
                    Repository.Add(newRegistration);
                }
            }

            await Repository.SaveChangesAsync();
        }

        private ContestRegistration CreateContestRegistrationForImportFromContest(ContestRegistration registration)
        {
            if (registration is TeamContestRegistration)
            {
                var source = (TeamContestRegistration) registration;
                var res = new TeamContestRegistration
                {
                    Participant2Id = source.Participant2Id,
                    Participant3Id = source.Participant3Id,
                    TeamName = source.TeamName,
                    OfficialTeamName = source.OfficialTeamName,
                };
                return res;
            }

            var src = (IndividualContestRegistration)registration;
            var indRes = new IndividualContestRegistration
            {
                Class = src.Class,
                Course = src.Course,
                StudentType = src.StudentType
            };
            return indRes;
        }
    }
}
