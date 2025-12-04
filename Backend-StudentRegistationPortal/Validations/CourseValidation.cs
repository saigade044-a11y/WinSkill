using System.ComponentModel.DataAnnotations;
using UseCaseFinalSubmission.Models;

namespace UseCaseFinalSubmission.Validations
{
    public class CourseValidation:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {

            var course = (Course)validationContext.ObjectInstance;

            // Capacity must be integer (extra check)
            if (!int.TryParse(course.Capacity.ToString(), out _))
            {
                return new ValidationResult("Course capacity must be a whole number (integer).");
            }

            // Capacity must be >= 1
            if (course.Capacity < 1)
            {
                return new ValidationResult("Course capacity must be greater than 0.");
            }

            // Capacity should not be less than enrolled
            if (course.Capacity < course.EnrolledCount)
            {
                return new ValidationResult("Capacity cannot be less than the enrolled count.");
            }

            return ValidationResult.Success;


        }
    }
}
