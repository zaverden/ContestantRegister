namespace ContestantRegister.Services.DomainServices.ContestRegistration
{
    public interface ITeamContestRegistration : IContestRegistration
    {
        string Participant2Id { get; set; }

        string Participant3Id { get; set; }

        string ReserveParticipantId { get; set; }

        string Trainer2Id { get; set; }

        string Trainer3Id { get; set; }

        string TeamName { get; set; }

        string OfficialTeamName { get; set; }
    }

    public interface ICreateTeamContestRegistration : ITeamContestRegistration
    {
    }

    public interface IEditTeamContestRegistration : ITeamContestRegistration
    {
    }
}
