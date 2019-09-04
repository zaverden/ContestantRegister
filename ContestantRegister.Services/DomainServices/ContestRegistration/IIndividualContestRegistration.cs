using ContestantRegister.Models;

namespace ContestantRegister.Services.DomainServices.ContestRegistration
{
    public interface ICreateIndividualContestRegistration : IIndividualContestRegistration
    {

    }
    public interface IEditIndividualContestRegistration : IIndividualContestRegistration
    {
        string ParticipantName { get; set; }
    }

    public interface IIndividualContestRegistration : IContestRegistration
    {
        int? Course { get; set; }

        int? Class { get; set; }

        StudentType? StudentType { get; set; }
    }
}
