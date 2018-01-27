using System.ComponentModel.DataAnnotations;

namespace ContestantRegister.Models
{
    public class School : StudyPlace
    {
        [Required]
        [MaxLength(50)]
        public string Email { get; set; }
    }
}
