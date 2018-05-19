using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace ContestantRegister.Models
{
    public class ContestAreaCompClass 
    {
        public int CompClassId { get; set; }
        [JsonIgnore]
        public CompClass CompClass { get; set; }

        [Range(1, 30)]
        public int UsedComputersCount { get; set; }

        public List<Computer> Computers { get; set; } = new List<Computer>();
    }
}
