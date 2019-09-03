namespace ContestantRegister.ViewModels.ListItem
{
    public abstract class SelectedListItemViewModelBase : ListItemViewModelBase
    {
        public bool Selected { get; set; }
    }

    public abstract class ListItemViewModelBase
    {
        public string Text { get; set; }
        public int Value { get; set; }
    }
}
