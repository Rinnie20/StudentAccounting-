using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAccounting.Models
{
    public class Subject
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Название предмета обязательно")]
        [Display(Name = "Название предмета")]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Курс")]
        public int? CourseId { get; set; }  // теперь nullable

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        public ICollection<SubjectGrade> SubjectGrades { get; set; } = new List<SubjectGrade>();
    }
}