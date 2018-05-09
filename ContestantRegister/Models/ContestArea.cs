namespace ContestantRegister.Models
{
    public class ContestArea : DomainObject
    {
        public int ContestId { get; set; }
        public int AreaId { get; set; }

        public Contest Contest { get; set; }
        public Area Area { get; set; }
    }
}
