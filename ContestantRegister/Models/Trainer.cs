using System.ComponentModel;

namespace ContestantRegister.Models
{
    [DisplayName("Тренер")]
    public class Trainer : ApplicationUser
    {
        public bool IsSchool { get; set; }
    }
}
