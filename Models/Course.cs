using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название курса")]
        [Display(Name = "Название курса")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Введите ФИО куратора")]
        [Display(Name = "Куратор")]
        public string CuratorName { get; set; } = string.Empty;

        [Display(Name = "Описание")]
        public string? Description { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Subject> Subjects { get; set; } = new List<Subject>();
    }
}