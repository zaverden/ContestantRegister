using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Features.Frontend.Home.ViewModels
{
    public class StudyPlaceDropdownItemViewModel
    {
        public int Id { get; set; }
        public int CityId { get; set; }
        public string ShortName { get; set; }
        public string FullName { get; set; }
        public string Type { get; set; }
    }
}
