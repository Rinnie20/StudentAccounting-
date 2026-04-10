using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Data;

namespace StudentAccounting.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;
        public HomeController(ApplicationDbContext context) => _context = context;

        public async Task<IActionResult> Index()
        {
            var courses = await _context.Courses.ToListAsync();
            var studentsCount = await _context.Students.CountAsync();
            var coursesCount = courses.Count;
            var grades = await _context.SubjectGrades.Where(g => g.Grade.HasValue).ToListAsync();
            double averageGrade = grades.Any() ? grades.Average(g => g.Grade!.Value) : 0;
            ViewBag.StudentsCount = studentsCount;
            ViewBag.CoursesCount = coursesCount;
            ViewBag.AverageGrade = averageGrade.ToString("0.00");
            return View(courses);
        }

        public async Task<IActionResult> CourseStudents(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            var students = await _context.Students.Where(s => s.CourseId == id).ToListAsync();
            ViewBag.Course = course;
            return View(students);
        }

        public async Task<IActionResult> CourseSubjects(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            var subjects = await _context.Subjects.Where(s => s.CourseId == id).ToListAsync();
            ViewBag.Course = course;
            return View(subjects);
        }

        public async Task<IActionResult> SubjectGrades(int subjectId)
        {
            var subject = await _context.Subjects.Include(s => s.Course).FirstOrDefaultAsync(s => s.Id == subjectId);
            if (subject == null) return NotFound();
            var grades = await _context.SubjectGrades.Include(g => g.Student).Where(g => g.SubjectId == subjectId).ToListAsync();
            ViewBag.Subject = subject;
            double avg = grades.Any(g => g.Grade.HasValue) ? grades.Where(g => g.Grade.HasValue).Average(g => g.Grade!.Value) : 0;
            ViewBag.AverageGrade = avg.ToString("0.00");
            return View(grades);
        }

        public async Task<IActionResult> StudentDetails(int id)
        {
            var student = await _context.Students
                .Include(s => s.Course)
                .Include(s => s.SubjectGrades)
                    .ThenInclude(g => g.Subject)
                .FirstOrDefaultAsync(s => s.Id == id);
            if (student == null) return NotFound();
            return View(student);
        }
    }
}