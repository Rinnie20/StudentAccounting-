using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentAccounting.Models
{
    public class SubjectGrade
    {
        [Key]
        public int Id { get; set; }
        public int StudentId { get; set; }
        [ForeignKey("StudentId")]
        public Student Student { get; set; } = null!;
        public int SubjectId { get; set; }
        [ForeignKey("SubjectId")]
        public Subject Subject { get; set; } = null!;
        [Range(2, 5)]
        public int? Grade { get; set; }
        public DateTime? DateGraded { get; set; }
    }
}