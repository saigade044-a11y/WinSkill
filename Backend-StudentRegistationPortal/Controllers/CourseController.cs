using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UseCaseFinalSubmission.Database;
using UseCaseFinalSubmission.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UseCaseFinalSubmission.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class CourseController : ControllerBase
    {
        private readonly Database1 database1;

        public CourseController(Database1 database1)
        {
            this.database1 = database1;
        }

        [HttpGet]
        [Route("GetAllCourses")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Course>>> GetAllCourses()
        {
            var courses = await database1.Course
                .Include(c => c.Enrollements)
                .Select(c => new
                {
                    c.CourseId,
                    c.CourseName,
                    c.CourseDescription,
                    c.Capacity,
                    EnrolledCount = c.Enrollements.Count(),
                    AvailableSeats = c.Capacity - c.Enrollements.Count()
                }).ToListAsync();

            return Ok(courses);
        }

        [HttpPost]
        [Route("CreateCourse")]
        public async Task<ActionResult<Course>> CreateCourses([FromBody] Course model)
        {

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new { messages = errors });

            }




            if (model == null)
               return BadRequest(new {message="Invalid Course Data" });

            var courseExists = await database1.Course.AnyAsync(n => n.CourseName.ToLower() == model.CourseName.ToLower());

            if (courseExists)
                return BadRequest(new { message = "Already The Course Exists" });

            model.EnrolledCount = 0;
            model.AvailableSeats = model.Capacity;

            database1.Course.Add(model);
            await database1.SaveChangesAsync();

            return Ok(new { Message = "Course created successfully", Course = model });
        }

        [HttpGet]
        [Route("GetCourseById/{id:int}")]
        public async Task<ActionResult<Course>> GetCourseById(int id)
        {
            if (id <= 0) return BadRequest("The Id Not Be Null");

            var course = await database1.Course.Where(n => n.CourseId == id).FirstOrDefaultAsync();

            if (course == null) return NotFound("The Course Is Not Found");

            return Ok(new { Message = "Course is Getting Based on Id", Course = course });
        }


        [HttpPut]
        [Route("UpdateCourseById/{id:int}")]
        public async Task<ActionResult<Course>> UpdateCourse([FromBody] Course model, int id)
        {
            if (id <= 0)
                return BadRequest(new { message = "The Id Value BE Not Less Than Zero" });



            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();

                return BadRequest(new { messages = errors });

            }




            var existingcourse = await database1.Course.Include(c => c.Enrollements).FirstOrDefaultAsync(n => n.CourseId == id);

            if (existingcourse == null)
                return NotFound(new { message = "No Course Has Found In It" });


            int enrolledCount = existingcourse.Enrollements.Count();

            // Prevent decreasing capacity below enrolled students
            if (model.Capacity < enrolledCount)
            {
                return BadRequest(new
                {
                    message = $"Capacity cannot be less than the number of enrolled students ({enrolledCount})."
                });
            }


            existingcourse.CourseName = model.CourseName;
            existingcourse.CourseDescription = model.CourseDescription;
            existingcourse.Capacity = model.Capacity;
            existingcourse.EnrolledCount = existingcourse.Enrollements.Count();
            existingcourse.AvailableSeats = existingcourse.Capacity - existingcourse.EnrolledCount;

            database1.Course.Update(existingcourse);
            await database1.SaveChangesAsync();

            return Ok(new { message = "Course updated successfully", Course = existingcourse });
        }



        [HttpDelete]
        [Route("DeleteById/{id:int}")]
        public async Task<ActionResult<Course>> DeleteByCourse(int id)
        {
            if (id <= 0)
                return BadRequest("The Id value is Not Less Than Zero");

            var course = await database1.Course.FirstOrDefaultAsync(n => n.CourseId == id);
            if (course == null)
                return BadRequest("No Course Has Found");

            database1.Remove(course);
            await database1.SaveChangesAsync();

            return Ok(new { Message = "Course deleted successfully", DeletedCourse = course });
        }
    }
}
