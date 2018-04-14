using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public enum ContestRegistrationStatus : int
    {
        [Display(Name = "Завершена")]
        Completed = 1,

        [Display(Name = "Подтверждение участия")]
        ConfirmParticipation = 2,

        [Display(Name = "Отменена")]
        Canceled = 3,
    }
}
