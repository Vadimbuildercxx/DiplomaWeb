using Diploma.Models;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Diploma.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Имя - это обязательное поле")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия - это обязательное поле")]
        public string? LastName { get; set; }

        public string? Patronymic { get; set; }

        [DataType(DataType.EmailAddress)]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Вы должны ввести телефон")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Вы должны ввести возраст")]
        public int? Age { get; set; }
        public string Gender{ get; set; }
    }
}
