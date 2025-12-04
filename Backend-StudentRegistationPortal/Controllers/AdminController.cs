using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Threading.Tasks;

using UseCaseFinalSubmission.Database;
using UseCaseFinalSubmission.Models;

namespace UseCase.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly Database1 _context;

        public AdminController(Database1 context)
        {
            _context = context;
        }


        [HttpGet("AllStudentEnrollments")]
        public async Task<ActionResult> GetAllStudentEnrollments()
        {
            var students = await _context.Student
                .Include(s => s.Enrollements)
                .ThenInclude(e => e.course)
                .Select(s => new
                {
                    s.StudentId,
                    StudentName = s.FullName,
                    StudentEmail = s.Email,
                    EnrolledCourses = s.Enrollements.Select(e => new
                    {
                        e.course.CourseId,
                        e.course.CourseName,
                        e.course.Capacity,
                        EnrolledCount = e.course.Enrollements.Count(),
                        AvailableSeats = e.course.Capacity - e.course.Enrollements.Count()
                    })
                }).ToListAsync();

            

            return Ok(students);
        }



        [HttpGet("EnrolledStudentsByCourseId/{courseId:int}")]
        public async Task<ActionResult> GetEnrolledStudentsByCourseId(int courseId)
        {
            var course = await _context.Course
                .Include(c => c.Enrollements)
                .ThenInclude(e => e.student)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);

            if (course == null)
                return NotFound("Course not found.");

            var students = course.Enrollements.Select(e => new
            {
                e.student.StudentId,
                e.student.FullName,
                e.student.Email,
                e.student.Phone,
                EnrolledOn = e.student.datetime,
            }).ToList();

            int enrolledCount = course.Enrollements.Count();
            int availableSeats = course.Capacity - enrolledCount;

            if (students.Count == 0)
                return Ok(new
                {
                    Course = course.CourseName,
                    Capacity = course.Capacity,
                    EnrolledCount = enrolledCount,
                    AvailableSeats = availableSeats,
                    Message = "No students enrolled for this course yet."
                });

            return Ok(new
            {
                Course = course.CourseName,
                Capacity = course.Capacity,
                EnrolledCount = enrolledCount,
                AvailableSeats = availableSeats,
                TotalStudents = students.Count,
                Students = students
            });
        }






        [HttpGet("AllStudents")]
        public async Task<ActionResult<IEnumerable<Student>>> GetAllStudents()
        {
            var students = await _context.Student.ToListAsync();
            return Ok(students);
        }



        [HttpPost]
        [Route("StudentRegister")]
        public async Task<ActionResult<Student>> CreateRegister([FromBody] Student student)
        {



            if (!ModelState.IsValid)
            {
                 var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                 return BadRequest(new { messages = errors });

            }
            var existingUser = await _context.Student.FirstOrDefaultAsync(n => n.Email.ToLower() == student.Email.ToLower() || n.Phone.ToLower() == student.Phone.ToLower());

            if (existingUser != null)
            {
                return BadRequest(new {message= "Already This User Has Present" });
            }





         /*  var passwordhasher = new PasswordHasher<Student>();

            var stringhashedpassword = passwordhasher.HashPassword(student, student.Password);
*/


             _context.Student.Add(student);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Student Regsitered Succesfully",
                Student = new
                {
                    student.StudentId,
                    student.Password,
                    student.FullName,
                    student.Email,
                    student.Phone,
                    student.Address,
                }
            });
        }






        [HttpPut("Update/{id:int}")]
        public async Task<ActionResult> UpdateStudent(int id, [FromBody] Student model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new { messages = errors });

            }


            if (id <= 0)
                return BadRequest("Invalid ID.");

            var existing = await _context.Student.FindAsync(id);

            if (existing == null)
                return NotFound("Student not found.");

            existing.FullName = model.FullName;
            existing.Email = model.Email;
            existing.Phone = model.Phone;
            existing.Password = model.Password;
            existing.Address = model.Address;




            _context.Student.Update(existing);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Student updated successfully", Student = existing });
        }



        [HttpDelete("Delete/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid ID.");

            var student = await _context.Student.FindAsync(id);
            if (student == null)
                return NotFound("Student not found.");

            _context.Student.Remove(student);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Student deleted successfully", DeletedStudent = student });
        }


        [HttpGet]
        [Route("ByEmail/{email}")]
        [AllowAnonymous]
        public async Task<ActionResult<Student>> getByData(string email)
        {
            if (string.IsNullOrEmpty(email))
                return BadRequest(new { Message = "Email is required" });

            var student = await _context.Student
                                        .Where(n => n.Email.ToLower() == email.ToLower())
                                        .FirstOrDefaultAsync();

            if (student == null)
                return NotFound(new { Message = "No student found for this email" });

            return Ok(student);
        }



        [HttpGet("GetCourseStats")]
        public async Task<IActionResult> GetCourseStats()
        {
            // Total courses
            var totalCourses = await _context.Course.CountAsync();

            // Total students
            var totalStudents = await _context.Student.CountAsync();

            // Total enrollments
            var totalEnrollments = await _context.Enrollements.CountAsync();

            // Students enrolled at least in 1 course
            var enrolledStudentIds = await _context.Enrollements
                .Select(e => e.StudentId)
                .Distinct()
                .ToListAsync();
            var totalEnrolledStudents = enrolledStudentIds.Count;

            // Students not enrolled
            var totalNotEnrolledStudents = totalStudents - totalEnrolledStudents;

            // Per-course enrollment details
            var courses = await _context.Course
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    c.Capacity,
                    c.EnrolledCount,
                    c.AvailableSeats,
                    NotEnrolledSeats = c.Capacity - c.EnrolledCount
                })
                .ToListAsync();

            var result = new
            {
                TotalCourses = totalCourses,
                TotalStudents = totalStudents,
                TotalEnrolledStudents = totalEnrolledStudents,
                TotalNotEnrolledStudents = totalNotEnrolledStudents,
                TotalEnrollments = totalEnrollments,
                Courses = courses
            };

            return Ok(result);
        }




    }
}
