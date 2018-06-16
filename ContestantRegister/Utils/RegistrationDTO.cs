namespace ContestantRegister.Utils
{
    public class ContestRegistrationDTO
    {
        //Эти поля маппятся из связанных объектов с регистрацией и их импортировать нельзя
        public string TrainerEmail { get; set; }
        public string TrainerName { get; set; }
        public string ManagerEmail { get; set; }
        public string ManagerName { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string StudyPlace { get; set; }

        //Для этого поля есть хак для маппинга 
        public string Area { get; set; }

        //Эти поля напрямую маппятся на регистрацию и их можно импортировать
        public string Status { get; set; }
        public string YaContestLogin { get; set; }
        public string YaContestPassword { get; set; }
        public int? Number { get; set; }
        public string ComputerName { get; set; }
        public string ProgrammingLanguage { get; set; }
    }
}
