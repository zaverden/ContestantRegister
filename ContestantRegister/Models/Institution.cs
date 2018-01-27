using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class Institution : StudyPlace
    {
        [Required]
        [MaxLength(50)]
        public string ShortNameEnglish { get; set; }

        [Required]
        [MaxLength(200)]
        public string FullNameEnglish { get; set; }

        [MaxLength(200)]
        public string BaylorLink { get; set; }
    }
}
