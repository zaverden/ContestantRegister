using System.Linq;
using System.Threading.Tasks;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Utils;
using ContestantRegister.Cqrs.Features.Frontend.Contests.Common.ViewModels;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;
using Microsoft.AspNetCore.Identity;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.CommandHandlers
{
    public abstract class CreateContestRegistrationCommandHandler<TCommand> : ICommandHandler<TCommand>
        where TCommand : ICommand
    {
        protected readonly IRepository Repository;
        private readonly IEmailSender _emailSender;
        private readonly ICurrentUserService _currentUserService;
        private readonly UserManager<ApplicationUser> _userManager;

        protected CreateContestRegistrationCommandHandler(
            IRepository repository, 
            IEmailSender emailSender, 
            ICurrentUserService currentUserService, 
            UserManager<ApplicationUser> userManager) 
        {
            Repository = repository;
            _emailSender = emailSender;
            _currentUserService = currentUserService;
            _userManager = userManager;
        }

        protected async Task FinishRegistrationAsync(ContestRegistrationViewModel viewModel, ContestRegistration registration, Contest contest)
        {
            registration.RegistrationDateTime = DateTimeService.SfuServerNow;
            registration.RegistredBy = await _userManager.FindByEmailAsync(_currentUserService.Email);
            registration.Status = ContestRegistrationStatus.Completed;

            var yacontestaccount = contest.YaContestAccountsCSV
                .SplitByNewLineEndAndRemoveWindowsLineEnds()
                .Skip(contest.UsedAccountsCount)
                .First()
                .Split(',');

            registration.YaContestLogin = yacontestaccount[0];
            registration.YaContestPassword = yacontestaccount[1];
            registration.Number = contest.RegistrationsCount + 1;

            contest.RegistrationsCount++;
            contest.UsedAccountsCount++;

            Repository.Add(registration);
            await Repository.SaveChangesAsync();

            //TODO Если регистрирует админ, то email не отправляется?
            if (contest.SendRegistrationEmail)
            {
                string email;
                if (contest.ContestType == ContestType.Individual)
                {
                    var participantEmail = await Repository.Set<ApplicationUser>()
                        .Where(u => u.Id == viewModel.Participant1Id)
                        .Select(x => x.Email)
                        .SingleAsync();
                    email = participantEmail;
                }
                else // contest.ContestType == ContestType.Team
                {
                    //Нужно ли отправлять email каждому члену команды?
                    email = _currentUserService.Email;
                }

                await _emailSender.SendEmailAsync(email,
                    "Вы зарегистрированы на соревнование по программированию ИКИТ СФУ",
                    $"Вы успешно зарегистрированы на соревнование: {contest.Name}<br>" +
                    $"Ваши учетные данные для входа в систему:<br>" +
                    $"логин {registration.YaContestLogin}<br>" +
                    $"пароль {registration.YaContestPassword}<br>" +
                    $"cсылка для входа: {contest.YaContestLink}<br>");
            }
        }

        public abstract Task HandleAsync(TCommand command);
    }
}
