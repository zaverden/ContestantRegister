namespace ContestantRegister.Models
{
    public enum ContestStatus : int
    {
        RegistrationClosed = 1,
        RegistrationOpened = 2,
        ConfirmParticipation = 3,
        ConfirmParticipationOrRegister = 4,
        Finished = 5
    }
}
