namespace ContestantRegister.ViewModels.Contest
{
    public class SortingViewModel
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public int SelectedContestAreaId { get; set; }

        public int[] SelectedCompClassIds { get; set; }        
    }
}
