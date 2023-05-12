using System.ComponentModel.DataAnnotations;

namespace Diploma.ViewModels
{
    public class PersonDTO
    {
        public string? FullName { get; set; }

        public string? Email { get; set; }

        [Required(ErrorMessage = "Вы должны ввести телефон")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Вы должны ввести возраст")]
        public int? Age { get; set; }
        public string Gender { get; set; }

        public List<string>? Paths { get; set; }
    }
}
