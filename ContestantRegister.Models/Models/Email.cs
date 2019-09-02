using System;
using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class Email : DomainObject
    {
        [Required]
        [StringLength(100)]
        [Display(Name = "Тема")]
        public string Subject { get; set; }

        [Required]
        [StringLength(100)]
        [Display(Name = "Адрес")]
        public string Address { get; set; }

        [Required]
        [StringLength(4000)]
        [DataType(DataType.MultilineText)]
        [Display(Name = "Сообщение")]
        public string Message { get; set; }

        [Required]
        [Display(Name = "Отправлено")]
        public bool IsSended { get; set; }

        [Display(Name = "Изменено")]
        public DateTime? ChangeDate { get; set; }

        [Display(Name = "Создано")]
        public DateTime? CreateDate { get; set; }

        [Display(Name = "Попыток")]
        public int SendAttempts { get; set; }
    }
}
