using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContestantRegister.Domain.Repository;
using ContestantRegister.Models;
using ContestantRegister.Services.Extensions;
using ContestantRegister.Services.InfrastructureServices;

namespace ContestantRegister.Services.DomainServices.ContestRegistration
{
    public interface IContestRegistrationService
    {
        Task<List<KeyValuePair<string, string>>> ValidateCreateIndividualContestRegistrationAsync(ICreateIndividualContestRegistration viewModel);
        Task<List<KeyValuePair<string, string>>> ValidateEditIndividualContestRegistrationAsync(IEditIndividualContestRegistration viewModel);

        Task<List<KeyValuePair<string, string>>> ValidateCreateTeamContestRegistrationAsync(ICreateTeamContestRegistration viewModel);
        Task<List<KeyValuePair<string, string>>> ValidateEditTeamContestRegistrationAsync(IEditTeamContestRegistration viewModel);
    }
}
