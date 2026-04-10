using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAccounting.Models
{
    public class Student
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите фамилию")]
        [Display(Name = "Фамилия")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите имя")]
        [Display(Name = "Имя")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите отчество")]
        [Display(Name = "Отчество")]
        public string MiddleName { get; set; } = string.Empty;

        [Display(Name = "Дата рождения")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [EmailAddress]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Курс (группа)")]
        public int? CourseId { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
    }
}