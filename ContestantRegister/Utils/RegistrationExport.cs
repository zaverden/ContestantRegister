namespace ContestantRegister.Utils
{
    public class IndividualRegistrationExport : RegistrationExport
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Patronymic { get; set; }
    }

    public class TeamRegistrationExport : RegistrationExport
    {
        
    }

    public class RegistrationExport
    {
        public string Trainer { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string StudyPlace { get; set; }
        public string Status { get; set; }
        public string YaContestLogin { get; set; }
        public string YaContestPassword { get; set; }
        public string Area { get; set; }
        public int? Number { get; set; }
        public string ComputerName { get; set; }
        public string ProgrammingLanguage { get; set; }
    }
}
