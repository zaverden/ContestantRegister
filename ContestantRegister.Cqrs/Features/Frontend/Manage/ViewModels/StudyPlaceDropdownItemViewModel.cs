using System;
using System.Collections.Generic;
using System.Text;

namespace ContestantRegister.Cqrs.Features.Frontend.Manage.ViewModels
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
