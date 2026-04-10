using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAccounting.Models
{
    public class Enrollment
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("Student")]
        public int StudentId { get; set; }
        public Student Student { get; set; } = null!;

        [ForeignKey("Course")]
        public int CourseId { get; set; }
        public Course Course { get; set; } = null!;

        [Display(Name = "Оценка")]
        [Range(2, 5, ErrorMessage = "Оценка должна быть от 2 до 5")]
        public int? Grade { get; set; }  // null – оценка ещё не выставлена

        [Display(Name = "Дата выставления")]
        [DataType(DataType.Date)]
        public DateTime? DateGraded { get; set; }
    }
}