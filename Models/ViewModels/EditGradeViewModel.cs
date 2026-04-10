using System.ComponentModel.DataAnnotations;

namespace StudentAccounting.Models.ViewModels
{
    public class EditGradeViewModel
    {
        public int GradeId { get; set; }
        public string StudentFullName { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        [Range(2, 5)]
        public int? Grade { get; set; }
    }
}