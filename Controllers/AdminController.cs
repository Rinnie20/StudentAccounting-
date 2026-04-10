using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentAccounting.Data;
using StudentAccounting.Models;

namespace StudentAccounting.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ---------- Студенты ----------
        public async Task<IActionResult> Students()
        {
            var students = await _context.Students.Include(s => s.Course).ToListAsync();
            return View(students);
        }

        public async Task<IActionResult> SearchStudents(string searchTerm)
        {
            var students = string.IsNullOrEmpty(searchTerm)
                ? await _context.Students.Include(s => s.Course).ToListAsync()
                : await _context.Students.Where(s => s.LastName.Contains(searchTerm) || s.FirstName.Contains(searchTerm) || (s.Email != null && s.Email.Contains(searchTerm))).Include(s => s.Course).ToListAsync();
            return PartialView("_StudentTable", students);
        }

        public async Task<IActionResult> CreateStudent()
        {
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateStudent(Student student)
        {
            if (ModelState.IsValid)
            {
                if (student.CourseId.HasValue && !await _context.Courses.AnyAsync(c => c.Id == student.CourseId.Value))
                {
                    ModelState.AddModelError("CourseId", "Выбранный курс не существует");
                    ViewBag.Courses = await _context.Courses.ToListAsync();
                    return View(student);
                }
                _context.Students.Add(student);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Students));
            }
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View(student);
        }

        public async Task<IActionResult> EditStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student == null) return NotFound();
            ViewBag.Courses = await _context.Courses.ToListAsync();
            ViewBag.OriginalCourseId = student.CourseId;
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> EditStudent(int id, Student student, int? originalCourseId)
        {
            if (id != student.Id) return NotFound();

            // Если курс не менялся, пароль не нужен
            if (student.CourseId != originalCourseId && originalCourseId != null)
            {
                string adminPassword = Request.Form["adminPassword"];
                if (adminPassword != "admin")
                {
                    ModelState.AddModelError("", "Неверный пароль администратора. Изменение курса отклонено.");
                    ViewBag.Courses = await _context.Courses.ToListAsync();
                    ViewBag.OriginalCourseId = originalCourseId;
                    return View(student);
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Students));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Students.Any(s => s.Id == student.Id)) return NotFound();
                    throw;
                }
            }
            ViewBag.Courses = await _context.Courses.ToListAsync();
            ViewBag.OriginalCourseId = originalCourseId;
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var student = await _context.Students.FindAsync(id);
            if (student != null)
            {
                _context.Students.Remove(student);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Students));
        }

        // ---------- Курсы ----------
        public async Task<IActionResult> Courses()
        {
            var courses = await _context.Courses.ToListAsync();
            return View(courses);
        }

        public IActionResult CreateCourse() => View();

        [HttpPost]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Courses.Add(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }
            return View(course);
        }

        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null) return NotFound();
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> EditCourse(int id, Course course)
        {
            if (id != course.Id) return NotFound();
            if (ModelState.IsValid)
            {
                _context.Update(course);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Courses));
            }
            return View(course);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course != null)
            {
                _context.Courses.Remove(course);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Courses));
        }

        // ---------- Предметы ----------
        public async Task<IActionResult> Subjects()
        {
            var subjects = await _context.Subjects.Include(s => s.Course).ToListAsync();
            return View(subjects);
        }

        public async Task<IActionResult> CreateSubject()
        {
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateSubject(Subject subject)
        {
            // Проверка, что курс выбран
            if (!subject.CourseId.HasValue)
            {
                ModelState.AddModelError("CourseId", "Выберите курс");
            }
            else if (!await _context.Courses.AnyAsync(c => c.Id == subject.CourseId.Value))
            {
                ModelState.AddModelError("CourseId", "Выбранный курс не существует");
            }

            if (ModelState.IsValid)
            {
                _context.Subjects.Add(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Subjects));
            }
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View(subject);
        }

        public async Task<IActionResult> EditSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject == null) return NotFound();
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> EditSubject(int id, Subject subject)
        {
            if (id != subject.Id) return NotFound();
            if (!subject.CourseId.HasValue)
            {
                ModelState.AddModelError("CourseId", "Выберите курс");
            }
            if (ModelState.IsValid)
            {
                _context.Update(subject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Subjects));
            }
            ViewBag.Courses = await _context.Courses.ToListAsync();
            return View(subject);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubject(int id)
        {
            var subject = await _context.Subjects.FindAsync(id);
            if (subject != null)
            {
                _context.Subjects.Remove(subject);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Subjects));
        }
    }
}