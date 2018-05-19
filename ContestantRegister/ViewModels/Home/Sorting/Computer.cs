using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContestantRegister.Models
{
    public class Computer 
    {
        [Range(1, 30)]
        public int Number { get; set; }

        public int? ContestRegistrationId { get; set; }

        [JsonIgnore]
        public ContestRegistration ContestRegistration { get; set; }
    }
}
