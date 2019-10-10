using System.Threading.Tasks;
using ContestantRegister.Application.Handlers.Frontend.Handlers.Home.Commands;
using ContestantRegister.Framework.Cqrs;
using ContestantRegister.Services.InfrastructureServices;
using ContestantRegister.Utils;
using Microsoft.Extensions.Options;

namespace ContestantRegister.Application.Handlers.Frontend.Handlers.Home.CommansHandlers
{
    public class SuggestSchoolCommandHandler : ICommandHandler<SuggestSchoolCommand>
    {
        private readonly IEmailSender _emailSender;
        private readonly SuggestStudyPlaceOptions _options;

        public SuggestSchoolCommandHandler(IEmailSender emailSender, IOptions<SuggestStudyPlaceOptions> options)
        {
            _emailSender = emailSender;
            _options = options.Value;
        }

        public async Task HandleAsync(SuggestSchoolCommand command)
        {
            var body = $"Предложил {command.ViewModel.Email}<br>" +
                       $"Краткое название {command.ViewModel.ShortName}<br>" +
                       $"Полное название {command.ViewModel.FullName}<br>" +
                       $"Регион {command.ViewModel.Region}<br>" +
                       $"Город {command.ViewModel.City}<br>" +
                       $"Официальный email {command.ViewModel.SchoolEmail}<br>" +
                       $"Сайт {command.ViewModel.Site}<br>";
            await _emailSender.SendEmailAsync(_options.Email, "Новая школа", body);
        }
    }
}
