using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Data;
using StudentAccounting.Models;
using StudentAccounting.Models.ViewModels;

namespace StudentAccounting.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ManagerController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> SelectCourse()
        {
            return View(await _context.Courses.ToListAsync());
        }

        public async Task<IActionResult> SelectSubject(int courseId)
        {
            var course = await _context.Courses.FindAsync(courseId);
            if (course == null) return NotFound();
            var subjects = await _context.Subjects.Where(s => s.CourseId == courseId).ToListAsync();
            ViewBag.Course = course;
            return View(subjects);
        }

        public async Task<IActionResult> SubjectGrades(int subjectId)
        {
            var subject = await _context.Subjects.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null) return NotFound();

            var grades = await _context.SubjectGrades.Include(g => g.Student).Where(g => g.SubjectId == subjectId).ToListAsync();
            var studentsInCourse = await _context.Students.Where(s => s.CourseId == subject.CourseId).ToListAsync();

            foreach (var s in studentsInCourse)
                if (!grades.Any(g => g.StudentId == s.Id))
                    grades.Add(new SubjectGrade { Student = s, Subject = subject, Grade = null, StudentId = s.Id, SubjectId = subjectId });

            ViewBag.Subject = subject;
            double avg = grades.Any(g => g.Grade.HasValue) ? grades.Where(g => g.Grade.HasValue).Average(g => g.Grade.Value) : 0;
            ViewBag.AverageGrade = avg.ToString("0.00");

            return View(grades);
        }

        [HttpGet]
        public async Task<IActionResult> EditGrade(int studentId, int subjectId)
        {
            var grade = await _context.SubjectGrades.FirstOrDefaultAsync(g => g.StudentId == studentId && g.SubjectId == subjectId);
            if (grade == null)
            {
                grade = new SubjectGrade { StudentId = studentId, SubjectId = subjectId };
                _context.SubjectGrades.Add(grade);
                await _context.SaveChangesAsync();
            }
            var student = await _context.Students.FindAsync(studentId);
            var subject = await _context.Subjects.FindAsync(subjectId);
            if (student == null || subject == null) return NotFound();
            var model = new EditGradeViewModel
            {
                GradeId = grade.Id,
                StudentFullName = $"{student.LastName} {student.FirstName} {student.MiddleName}",
                SubjectName = subject.Name,
                Grade = grade.Grade
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditGrade(EditGradeViewModel model)
        {
            if (ModelState.IsValid)
            {
                var grade = await _context.SubjectGrades.FindAsync(model.GradeId);
                if (grade != null)
                {
                    grade.Grade = model.Grade;
                    grade.DateGraded = model.Grade.HasValue ? DateTime.Now : null;
                    await _context.SaveChangesAsync();
                }
                return RedirectToAction(nameof(SubjectGrades), new { subjectId = grade?.SubjectId });
            }
            return View(model);
        }
    }
}