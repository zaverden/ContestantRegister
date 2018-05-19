using System.Collections.Generic;
using ContestantRegister.Models;

namespace ContestantRegister.ViewModels.Home
{
    public class SortingViewModel
    {
        public Contest Contest { get; set; }

        public int? SelectedAreaId { get; set; }

        public int[] SelectedCompClassIds { get; set; }

        public List<ContestAreaCompClass> ContestAreaCompClasses = new List<ContestAreaCompClass>();
    }
}
