using System.ComponentModel;

namespace ContestantRegister.Models
{
    [DisplayName("Тренер")]
    public class Trainer : ContestantUser
    {
        public bool IsSchool { get; set; }
    }
}
