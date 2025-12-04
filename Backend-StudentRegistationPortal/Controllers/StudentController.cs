using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UseCaseFinalSubmission.Database;
using UseCaseFinalSubmission.Models;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace UseCaseFinalSubmission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Student")]

    public class StudentController : ControllerBase
    {
        private readonly Database1 database;

        private readonly ILogger<StudentController> logger;

        public StudentController(Database1 database, ILogger<StudentController> logger)
        {
            this.database = database;
            this.logger = logger;
        }


        [HttpPost]
        [Route("EnrollCourse")]
        public async Task<ActionResult> EnrollCourse([FromBody] EnrollAndUnEroll model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentdata = await database.Student.FirstOrDefaultAsync(n => n.StudentId == model.StudentId1);

            var coursedata = await database.Course.Include(n => n.Enrollements).FirstOrDefaultAsync(n => n.CourseId == model.CourseId1);

            if (studentdata == null || coursedata == null)
            {
                return BadRequest("The Data Has Not Found");
            }
            int enrolledCount = coursedata.Enrollements.Count();
            int availableSeats = coursedata.Capacity - coursedata.EnrolledCount;

            if (availableSeats <= 0)
                return BadRequest($"Course '{coursedata.CourseName}' is already full.");

            bool alreadyEnrolled = await database.Enrollements.AnyAsync(n => n.StudentId == model.StudentId1 && n.CourseId == model.CourseId1);

            if (alreadyEnrolled)
                return BadRequest(new {message="Already This Student Enrolled This Course"});

            Enrollement obj = new Enrollement()
            {
                StudentId = studentdata.StudentId,
                CourseId = coursedata.CourseId
            };

            database.Enrollements.Add(obj);
            coursedata.EnrolledCount++;
            coursedata.AvailableSeats = coursedata.Capacity - coursedata.EnrolledCount;

            database.Course.Update(coursedata);
            await database.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Student '{studentdata.FullName}' successfully enrolled in '{coursedata.CourseName}'.",
                Course = new
                {
                    coursedata.CourseId,
                    coursedata.CourseName,
                    coursedata.Capacity,
                    coursedata.EnrolledCount,
                    coursedata.AvailableSeats
                }
            });
        }




        [HttpPost]
        [Route("UnEnrolleCourse")]
        public async Task<ActionResult> UnEnrollCourse([FromBody] EnrollAndUnEroll model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var studentdata = await database.Student.FirstOrDefaultAsync(n => n.StudentId == model.StudentId1);
            var coursedata = await database.Course.Include(n => n.Enrollements).FirstOrDefaultAsync(n => n.CourseId == model.CourseId1);

            if (studentdata == null || coursedata == null)
                return BadRequest("The Data Has Not Found");

            var checking = await database.Enrollements.FirstOrDefaultAsync(n => n.StudentId == model.StudentId1 && n.CourseId == model.CourseId1);

            if (checking == null)
                return BadRequest("Student is not enrolled in this course.");

            database.Enrollements.Remove(checking);

            coursedata.EnrolledCount = coursedata.Enrollements.Count() - 1;
            if (coursedata.EnrolledCount <= 0)
                coursedata.EnrolledCount = 0;

            coursedata.AvailableSeats = coursedata.Capacity - coursedata.EnrolledCount;

            database.Course.Update(coursedata);
            await database.SaveChangesAsync();

            return Ok(new
            {
                Message = $"Student '{studentdata.FullName}' successfully unrolled in '{coursedata.CourseName}'.",
                Course = new
                {
                    coursedata.CourseId,
                    coursedata.CourseName,
                    coursedata.Capacity,
                    coursedata.EnrolledCount,
                    coursedata.AvailableSeats
                }
            });
        }




        [HttpGet]
        [Route("MyEnrollements/{StudentId:int}")]
        [AllowAnonymous]
        public async Task<ActionResult> GetMyEnrollements(int StudentId)
        {
            if (StudentId <= 0)
                return BadRequest("The id is Not Less Than The Zero");

            var student = await database.Student
                .Include(n => n.Enrollements)
                .ThenInclude(n => n.course)
                .FirstOrDefaultAsync(n => n.StudentId == StudentId);

            if (student == null)
                return BadRequest("The Student Details Was Not Found");

            var enrollment = student.Enrollements.Select(n => new
            {
                n.course.CourseName,
                n.course.CourseDescription,
                n.course.CourseId,
                n.course.Capacity,
                EnrolledCount = n.course.Enrollements.Count(),
                AvailableSeats = n.course.Capacity - n.course.Enrollements.Count()
            }).ToList();

            if (enrollment.Count == 0)
                return Ok(new { Message = "This student has not enrolled in any courses yet." });

            return Ok(new
            {
                Student = student.FullName,
                TotalCourses = enrollment.Count,
                EnrolledCourses = enrollment
            });
        }




    }
}
