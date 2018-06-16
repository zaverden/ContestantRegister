using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContestantRegister.Controllers
{
    public class ContestParticipantFilter
    {
        public string ParticipantName { get; set; }
        public string TrainerName { get; set; }
        public string ManagerName { get; set; }
        public string City { get; set; }
        public string Area { get; set; }
        public string Status { get; set; }
        public string StudyPlace { get; set; }
    }
}
