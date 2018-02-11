using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public enum ContestType : int
    {
        [Display(Name = "Личный")]
        Individual = 1,

        [Display(Name = "Командный")]
        Collegiate = 2
    }
}
