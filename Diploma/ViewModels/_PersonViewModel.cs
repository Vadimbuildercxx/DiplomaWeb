using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using System.Xml.Linq;


namespace Diploma.ViewModels
{
    public enum PersonGender
    {
        [EnumMember(Value = "Male")]
        Male,
        [EnumMember(Value = "Female")]
        Female
    }

    public class PersonViewModel
    {
        [Required(ErrorMessage = "Имя - это обязательное поле")]
        [Display(Name = "Имя")]
        [DefaultValue(typeof(string), "Иван")]
        public string? FirstName { get; set; }

        [Required(ErrorMessage = "Фамилия - это обязательное поле")]
        [Display(Name = "Фамилия")]
        [DefaultValue(typeof(string), "Иванович")]
        public string? LastName { get; set; }

        [Display(Name = "Отчество")]
        [DefaultValue(typeof(string), "Иванов")]
        public string? Patronymic { get; set; }

        [DataType(DataType.EmailAddress)]
        [DefaultValue(typeof(string), "ivanov@mail.ru")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Вы должны ввести телефон")]
        [Display(Name = "Телефон")]
        [DataType(DataType.PhoneNumber)]
        [DefaultValue(typeof(string), "9001234567")]
        [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Неверный телефонный номер")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Вы должны ввести возраст")]
        [Display(Name = "Возраст")]
        [DefaultValue(typeof(int), "31")]
        public int? Age { get; set; }

        [Required(ErrorMessage = "Вы должны ввести пол")]
        [Display(Name = "Пол")]
        public string Gender { get; set; }

        public static string[] Genders = new[] { "Male", "Female", };

        public PersonViewModel() 
        {
            FirstName = "Иван";
            LastName = "Иванович";
            Patronymic = "Иванович";
            Email = "ivanov@mail.ru";
            Phone = "9001234567";
            Age = 31;
            Gender = Genders[0];
        }

        public string Info()
        {
            return FirstName + " " + LastName + " " + Patronymic + " " + Email + " " + Phone + " " + Gender.ToString();
        }
    }
}
