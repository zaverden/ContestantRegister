using ContestantRegister.Framework.Cqrs;

namespace ContestantRegister.Cqrs.Features.Frontend.Contests.Common.Commands
{
    public class DeleteRegistrationCommand : ICommand
    {
        public int RegistrationId { get; set; }
        public int ContestId { get; set; }
    }
}
